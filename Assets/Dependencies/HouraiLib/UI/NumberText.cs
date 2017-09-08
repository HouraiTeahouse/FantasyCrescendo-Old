using System;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {

    /// <summary> 
    /// Displays a number to a UnityEngine.UI.Text UI object. 
    /// </summary>
    [ExecuteInEditMode]
    public class NumberText : MonoBehaviour {

        [SerializeField]
        [Tooltip("The sring format used to display")]
        string _format;

        [SerializeField]
        [Tooltip("The number to display using this script")]
        float _number;

        [SerializeField]
        [Tooltip("The Text UI object driven by this script")]
        Text _text;

        /// <summary> 
        /// The number to be displayed by the Text UI object. 
        /// </summary>
        public float Number {
            get { return _number; }
            set {
                _number = value;
                UpdateText();
            }
        }

        /// <summary> 
        /// The string format used to display the number. 
        /// </summary>
        public string Format {
            get { return _format; }
            set {
                bool changed = _format == value;
                _format = value;
                if (changed)
                    UpdateText();
            }
        }

        /// <summary> 
        /// The Text UI object that is driven by this script. 
        /// </summary>
        protected Text Text {
            get { return _text; }
            set {
                bool changed = _text == value;
                _text = value;
                if (changed)
                    UpdateText();
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (Text == null)
                Text = GetComponent<Text>();
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() { 
            Text = GetComponent<Text>(); 
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update() {
            if (!Application.isPlaying)
                UpdateText();
        }

        protected virtual void UpdateText() {
            if (_text == null)
                return;
            _text.text = ProcessNumber(string.IsNullOrEmpty(_format) ? _number.ToString() : _number.ToString(_format));
        }

        protected virtual string ProcessNumber(string number) { 
            return number; 
        }

    }

}
