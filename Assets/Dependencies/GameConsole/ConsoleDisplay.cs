using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.Console {
    /// <summary>
    ///     Displays the most recent logs in the GameConsole on a Text object.
    ///     REQUIRED COMPONENT: UnityEngine.UI.Text
    /// </summary>
    [RequireComponent(typeof (Text))]
    public class ConsoleDisplay : MonoBehaviour {
        // The Text component used to render the console history
        private Text _displayText;

        // Used to build the log string from the console's history
        private StringBuilder _textBuilder;

        // Has the Console been updated since the log was last rendered?
        private bool _updated;

        /// <summary>
        ///     Unity Callback. Called on object instantiation.
        /// </summary>
        private void Awake() {
            _textBuilder = new StringBuilder();
            _updated = false;
            GameConsole.OnConsoleUpdate += Redraw;
            _displayText = GetComponent<Text>();
        }

        /// <summary>
        ///     Unity Callback. Called on object destruction.
        /// </summary>
        private void OnDestroy() {
            GameConsole.OnConsoleUpdate -= Redraw;
        }

        /// <summary>
        ///     Unity Callback. Called each time the Behaviour is enabled or the GameObject it is attached to is activated.
        /// </summary>
        private void OnEnable() {
            if (_updated)
                Redraw();
            _updated = false;
        }

        /// <summary>
        ///     Re-reads history and renders it to the Text component.
        /// </summary>
        private void Redraw() {
            if (!isActiveAndEnabled) {
                _updated = true;
                return;
            }
            // Clears the current string
            _textBuilder.Length = 0;
            foreach (var log in GameConsole.History)
                _textBuilder.AppendLine(log);
            _displayText.text = _textBuilder.ToString();
        }
    }
}