using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.Console {
    /// <summary>
    ///     UI Element to toggle the appearance of the GameConsole UI.
    /// </summary>
    public class ConsoleToggle : MonoBehaviour {
        [SerializeField, Tooltip("The KeyCode for the key that toggles the appearance of the GameConsole UI.")] private readonly KeyCode _key = KeyCode.F5;

        [SerializeField, Tooltip("GameObjects to activate and deactivate when toggling the GameConsole UI.")] private
            GameObject[] _toggle;

        [SerializeField, Tooltip("GameObject to selet when GameConsole is set to show")] private GameObject select;

        /// <summary>
        ///     Unity callback. Called once every frame.
        /// </summary>
        private void Update() {
            if (!Input.GetKeyDown(_key))
                return;
            foreach (var go in _toggle) {
                if (!go)
                    continue;
                go.SetActive(!go.activeSelf);
            }
            var eventSystem = EventSystem.current;
            if (eventSystem)
                eventSystem.SetSelectedGameObject(select);
        }
    }
}