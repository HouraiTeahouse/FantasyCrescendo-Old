using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary> 
    /// Abstract base class for EditorWindows that has the small padlock button at 
    /// the top like the Inspector. The state of the /// padlock can be accessed 
    /// through the IsLocked property. 
    /// </summary>
    public abstract class LockableEditorWindow : EditorWindow, IHasCustomMenu {

        GUIStyle lockButtonStyle;
        bool locked;

        /// <summary> 
        /// Gets or sets whether the EditorWindow is currently locked or not. 
        /// </summary>
        public bool IsLocked {
            get { return locked; }
            set { locked = value; }
        }

        public virtual void AddItemsToMenu(GenericMenu menu) {
            menu.AddItem(new GUIContent("Lock"), locked, () => { locked = !locked; });
        }

        void ShowButton(Rect position) {
            if (lockButtonStyle == null)
                lockButtonStyle = "IN LockButton";
            locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
        }

    }

}