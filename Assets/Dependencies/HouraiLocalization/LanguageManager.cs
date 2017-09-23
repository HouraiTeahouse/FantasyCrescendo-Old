using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace HouraiTeahouse.Localization {

    public class LanguageChanged {

        public Language NewLanguage;

    }

    public interface ILanguageStorageDelegate {
        event Action<string> OnChangeLangauge;

        string GetStoredLangaugeId(string defaultLangauge);
        void SetStoredLanguageId(string language);
    }

    public class PlayerPrefLanguageStorageDelegate : ILanguageStorageDelegate {

        readonly string _playerPrefKey;

        public event Action<string> OnChangeLangauge;

        public PlayerPrefLanguageStorageDelegate(string key) {
            _playerPrefKey = Argument.NotNull(key);
        }

        public string GetStoredLangaugeId(string defaultLangauge) {
            if (!Prefs.Exists(_playerPrefKey)) 
                SetStoredLanguageId(defaultLangauge);
            return Prefs.GetString(_playerPrefKey);
        }

        public void SetStoredLanguageId(string language) {
            if (Prefs.Exists(_playerPrefKey) || Prefs.GetString(_playerPrefKey) != language) {
                Prefs.SetString(_playerPrefKey, language);
                OnChangeLangauge?.Invoke(language);
            }
        }

    }

    /// <summary> 
    /// Singleton MonoBehaviour that manages all of localization system. 
    /// </summary>
    public sealed class LanguageManager : Singleton<LanguageManager> {

        internal static readonly ILog log = Log.GetLogger("Language");
        public const string FileExtension = ".json";
        Language _currentLanguage;
        string _storageDirectory;
        HashSet<string> _languages;

        static ILanguageStorageDelegate _storageDelegate;

        static LanguageManager() {
            Storage = new PlayerPrefLanguageStorageDelegate("lang");
        }

        static void StorageChangeLangauge(string language) {
            if (Instance == null)
                return;
            Instance.LoadLanguage(language);
        }

        public static ILanguageStorageDelegate Storage {
            get { return _storageDelegate; }
            set { 
                Argument.NotNull(value);
                if (_storageDelegate != null)
                    _storageDelegate.OnChangeLangauge -= StorageChangeLangauge;
                _storageDelegate = value;
                if (_storageDelegate != null)
                    _storageDelegate.OnChangeLangauge += StorageChangeLangauge;
            }
        }

#if HOURAI_EVENTS
        Mediator _eventManager;
#endif

        [SerializeField]
        [Tooltip("The default language to use if the Player's current language is not supported")]
        string _defaultLanguage = "en";

        [SerializeField]
        [Tooltip("Destroy this object on scene changes?")]
        bool _dontDestroyOnLoad = false;

        [SerializeField]
        [Tooltip("The Resources directory to load the Language files from")]
        string localizationDirectory = "lang";

        /// <summary> The currently used language. </summary>
        public Language CurrentLangauge => _currentLanguage;

        /// <summary> All available languages currently supported by the system. </summary>
        public IEnumerable<string> AvailableLanguages {
            get {
                if (_languages != null)
                    return _languages;
                return Enumerable.Empty<string>();
            }
        }

        /// <summary> Gets an enumeration of all of the localizable keys. </summary>
        public IEnumerable<string> Keys => CurrentLangauge.Keys;

        /// <summary> Localizes a key based on the currently loaded language. </summary>
        /// <param name="key"> the localization key to use. </param>
        /// <returns> the localized string </returns>
        public string this[string key] => CurrentLangauge[key];

        /// <summary> An event that is called every time the language is changed. </summary>
        public event Action<Language> OnChangeLanguage;

        void SetLanguage(string langName, IDictionary<string, string> values) {
            if (_currentLanguage.Name == langName)
                return;
            _currentLanguage.Update(values);
            _currentLanguage.Name = langName;
            OnChangeLanguage?.Invoke(_currentLanguage);
#if HOURAI_EVENTS
            _eventManager.Publish(new LanguageChanged {NewLanguage = _currentLanguage});
#endif
            log.Info("Set language to {0}", Language.GetName(langName));
        }

        string GetLanguagePath(string identifier) {
            return Path.Combine(_storageDirectory, identifier + FileExtension);
        }

        protected override void Awake() {
            base.Awake();

            _currentLanguage = new Language();
#if HOURAI_EVENTS
            _eventManager = Mediator.Global;
#endif

            _storageDirectory = Path.Combine(Application.streamingAssetsPath, localizationDirectory);
            var languages = Directory.GetFiles(_storageDirectory);
            _languages = new HashSet<string>(from file in languages
                                             where file.EndsWith(FileExtension)
                                             select Path.GetFileNameWithoutExtension(file));

            SystemLanguage systemLang = Application.systemLanguage;
            string currentLang = Storage.GetStoredLangaugeId(systemLang.ToIdentifier());
            if (!_languages.Contains(currentLang) || systemLang == SystemLanguage.Unknown) {
                log.Info("No language data for \"{0}\" found. Loading default language: {1}", _defaultLanguage, currentLang);
                currentLang = _defaultLanguage;
            }
            LoadLanguage(currentLang);
            Storage.SetStoredLanguageId(currentLang);

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        /// <summary> Unity Callback. Called on object destruction. </summary>
        void OnDestroy() { Save(); }

        /// <summary> Unity Callback. Called when entire application exits. </summary>
        void OnApplicationQuit() { Save(); }

        /// <summary> Saves the current language preferences to PlayerPrefs to keep it persistent. </summary>
        void Save() {
            Storage.SetStoredLanguageId(CurrentLangauge.Name);
        }

        /// <summary> Loads a new language given the Microsoft language identifier. </summary>
        /// <param name="identifier"> the Microsoft identifier for a lanuguage </param>
        /// <returns> the localization language </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="identifier" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> the language specified by <paramref name="identifier" /> is not currently
        /// supported. </exception>
        public Language LoadLanguage(string identifier) {
            Argument.NotNull(identifier);
            identifier = identifier.ToLower();
            if (!_languages.Contains(identifier))
                throw new InvalidOperationException("Language with identifier of {0} is not supported.".With(identifier));
            var languageValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(GetLanguagePath(identifier)));
            SetLanguage(identifier, languageValues);
            return CurrentLangauge;
        }

    }

}
