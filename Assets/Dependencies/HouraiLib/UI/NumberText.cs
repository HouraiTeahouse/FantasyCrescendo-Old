using System;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {
    /// <summary>
    ///     Displays a number to a UnityEngine.UI.Text UI object.
    /// </summary>
    [ExecuteInEditMode]
    public class NumberText : MonoBehaviour {
        [SerializeField, Tooltip("The string format used to display")] private string _format;

        [SerializeField, Tooltip("The number to display using this script")] private float _number;
        [SerializeField, Tooltip("The Text UI object driven by this script")] private Text _text;

        /// <summary>
        ///     The number to be displayed by the Text UI object.
        /// </summary>
        public float Number {
            get { return _number; }
            set {
                var changed = _number == value;
                _number = value;
                if (!changed)
                    return;
                if (OnNumberChange != null)
                    OnNumberChange();
                UpdateText();
            }
        }

        /// <summary>
        ///     The string format used to display the number.
        /// </summary>
        public string Format {
            get { return _format; }
            set {
                var changed = _format == value;
                _format = value;
                if (changed)
                    UpdateText();
            }
        }

        /// <summary>
        ///     The Text UI object that is driven by this script.
        /// </summary>
        protected Text Text {
            get { return _text; }
            set {
                var changed = _text == value;
                _text = value;
                if (changed)
                    UpdateText();
            }
        }

        public event Action OnNumberChange;

        /// <summary>
        ///     Unity Callback. Called on object instantiation.
        /// </summary>
        private void Awake() {
            if (Text == null)
                Text = GetComponent<Text>();
        }

        /// <summary>
        ///     Unity Callback. Called when the script is added in the Editor or the Reset menu option is selected.
        /// </summary>
        private void Reset() {
            Text = GetComponent<Text>();
        }

        /// <summary>
        ///     Unity Callback. Called once per frame.
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