using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HouraiTeahouse.UI {

    /// <summary>
    /// Sets a UI Text or <see cref="ITextAcceptor"/> component's text to <see cref="UnityEngine.Application.version"/>.
    /// A custom display format can be provided.
    /// Utilizes <see cref="ITextAcceptor.SetText(string)"/> to set the text of the object.
    /// </summary>
    public class VersionText : MonoBehaviour {

        [SerializeField]
        [Tooltip("Text object to assign the value to.")]
        GameObject _text;

        [SerializeField]
        [Tooltip("Custom display format. Use {0} to subsitute in the version text.")]
        string _displayFormat = "v{0}";

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (_text == null)
                _text = gameObject;
            // Do not show on non-debug builds
            if (!Debug.isDebugBuild) {
              DestroyImmediate(_text);
              return;
            }
            if (string.IsNullOrEmpty(_displayFormat))
                _text.SetUIText(Application.version);
            else
                _text.SetUIText(_displayFormat.With(Application.version));
            Destroy(this);
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() {
            _text = gameObject;
        }

    }

}
