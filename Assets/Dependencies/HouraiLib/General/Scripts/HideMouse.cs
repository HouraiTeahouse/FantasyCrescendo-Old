using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    ///     Hides the cursor
    /// </summary>
    public class HideMouse : MonoBehaviour {
        /// <summary>
        ///     Whether the cursor hiding works in the editor or not.
        /// </summary>
        [SerializeField] private bool _inEditor;

#if UNITY_EDITOR
        private void OnEnable() {
            if (_inEditor)
                Cursor.visible = false;
        }

        private void OnDisable() {
            if (_inEditor)
                Cursor.visible = true;
        }
#else
        void OnEnable() {
            Cursor.visible = false;
        }

        void OnDisable() {
            Cursor.visible = true;
        }
#endif
    }
}