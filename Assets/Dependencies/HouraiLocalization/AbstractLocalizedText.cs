using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HouraiTeahouse.Localization {

    /// <summary> 
    /// An abstract MonoBehaviour class that localizes the strings displayed on UI Text objects. 
    /// </summary>
    public abstract class AbstractLocalizedText : MonoBehaviour, ITextAcceptor {

        [SerializeField]
        Text _text;

        [SerializeField]
        TMP_Text _textMesh;

        string _nativeText;

        public int Priority => 100;

        /// <summary> 
        /// The UI Text object to display the localized string onto 
        /// </summary>
        public Text Text {
            get { return _text; }
            set { 
                _text = value; 
                SetText();
            }
        }

        public TMP_Text TextMesh {
            get { return _textMesh; }
            set { 
                _textMesh = value; 
                SetText();
            }
        }

        /// <summary> 
        /// The localization key used to lookup the localized string. 
        /// </summary>
        protected string NativeText {
            get { return _nativeText; }
            set {
                _nativeText = value;
                if (value == null || !HasComponent())
                    return;
                SetText();
            }
        }

        protected bool HasComponent() => _text || _textMesh;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake() {
            if (!HasComponent())
                ResetComponents();
            enabled = HasComponent();
            var context = Mediator.Global.CreateUnityContext(this);
            context.Subscribe<LanguageChanged>(OnChangeLanguage);
        }

        protected void ResetComponents() {
            if (!HasComponent())
                _text = GetComponent<Text>();
            if (!HasComponent())
                _textMesh = GetComponent<TextMeshProUGUI>();
        }

        protected void SetText(string text = null) {
            if (string.IsNullOrEmpty(text)) {
                LanguageManager languageManager = LanguageManager.Instance;
                if (languageManager)
                    text = Process(languageManager.CurrentLanguage[NativeText]);
                else
                    text = string.Empty;
            }
            text = Process(text);
            if (Text)
                Text.text = text;
            if (TextMesh)
                TextMesh.text = text;
        }

        protected string GetText() {
            string text = string.Empty;
            if (Text != null)
                text = Text.text;
            if (TextMesh != null)
                text = TextMesh.text;
            return text;
        }

        protected virtual void Reset() => ResetComponents();

        /// <summary> Unity Callback. Called on the first frame before Update is called. </summary>
        protected virtual void Start() {
            SetText();
        }

        /// <summary> 
        /// Events callback for when the system wide language is changed. 
        /// </summary>
        /// <param name="args"> the language set that was changed to. </param>
        void OnChangeLanguage(LanguageChanged args) {
            if (args == null || NativeText == null)
                return;
            SetText(args.Language[NativeText]);
        }

        /// <summary> 
        /// Post-Processing on the retrieved localized string. 
        /// </summary>
        /// <param name="val"> the pre-processed localized string </param>
        /// <returns> the post-processed localized string </returns>
        protected virtual string Process(string val) => val;

        void ITextAcceptor.SetText(string text) {
            NativeText = text;
        }

    }
}