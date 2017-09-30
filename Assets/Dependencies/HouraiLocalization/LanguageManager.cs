using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace HouraiTeahouse.Localization {

    public class LanguageChanged {
        public LanguageManager Manager;
        public Language Language;
    }

    public interface ILanguageStorage {
        event Action<string> OnChangeLangauge;

        string GetStoredLangaugeId(string defaultLangauge);
        void SetStoredLanguageId(string language);
    }

    public class PlayerPrefLanguageStorageDelegate : ILanguageStorage {

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

        internal static readonly ILog _log = Log.GetLogger("Language");
        public const string FileExtension = ".json";
        string _storageDirectory;
        HashSet<string> _languages;

        static ILanguageStorage _storageDelegate;

        static LanguageManager() {
            Storage = new PlayerPrefLanguageStorageDelegate("lang");
        }

        static void StorageChangeLangauge(string language) {
            if (Instance == null)
                return;
            Instance.LoadLanguage(language);
        }

        public static ILanguageStorage Storage {
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

        [SerializeField]
        [Tooltip("The default language to use if the Player's current language is not supported")]
        string _defaultLanguage = "en";

        [SerializeField]
        [Tooltip("Destroy this object on scene changes?")]
        bool _dontDestroyOnLoad = false;

        [SerializeField]
        [Tooltip("The Resources directory to load the Language files from")]
        string localizationDirectory = "lang";

        /// <summary> 
        /// The currently used language. 
        /// </summary>
        public Language CurrentLanguage { get; private set; }

        /// <summary> 
        /// All available languages currently supported by the system. 
        /// </summary>
        public IEnumerable<string> AvailableLanguages => _languages.EmptyIfNull();

        /// <summary> 
        /// Gets an enumeration of all of the localizable keys. 
        /// </summary>
        public IEnumerable<string> Keys => CurrentLanguage.Keys;

        void SetLanguage(string langName, IDictionary<string, string> values) {
            if (CurrentLanguage.Name == langName)
                return;
            CurrentLanguage.Update(values);
            CurrentLanguage.Name = langName;
            Mediator.Global.Publish(new LanguageChanged {
                Manager = this,
                Language = CurrentLanguage
            });
            _log.Info("Set language to {0}", Language.GetName(langName));
        }

        string GetLanguagePath(string identifier) => 
            Path.Combine(_storageDirectory, identifier + FileExtension);

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();

            CurrentLanguage = new Language();
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
                _log.Info("No language data for \"{0}\" found. Loading default language: {1}", _defaultLanguage, currentLang);
                currentLang = _defaultLanguage;
            }
            LoadLanguage(currentLang);
            Storage.SetStoredLanguageId(currentLang);

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() => Save();

        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        void OnApplicationQuit() => Save();

        /// <summary> 
        /// Saves the current language preferences to PlayerPrefs to keep it persistent. 
        /// </summary>
        void Save() => Storage.SetStoredLanguageId(CurrentLanguage.Name);

        /// <summary> 
        /// Loads a new language given the Microsoft language identifier. 
        /// </summary>
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
            return CurrentLanguage;
        }

    }

}
