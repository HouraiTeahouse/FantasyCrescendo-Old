using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> 
    /// Hides the mouse cursor if setup properly.
    /// </summary>
    public class HideMouse : MonoBehaviour {

        /// <summary></summary>
        [SerializeField]
        [Tooltip("Whether the cursor hiding works in the editor or not.")]
        bool _inEditor;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
#if UNITY_EDITOR
            if (_inEditor)
#endif
                Cursor.visible = false;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
#if UNITY_EDITOR
            if (_inEditor)
#endif
                Cursor.visible = true;
        }

    }

}