using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.Editor;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HouraiTeahouse {

    /// <summary> 
    /// Abstract base class for EditorWindows that are reliant on the current selection of objects. 
    /// Inherits from LockableEditorWindow, the selection can be locked via the padlock in the 
    /// top right of the window. 
    /// </summary>
    /// <typeparam name="T"> the type of object to filter the selection for </typeparam>
    public abstract class SelectableEditorWindow<T> : LockableEditorWindow where T : Object {

        T[] _selection;

        /// <summary> 
        /// A list of selected objects. 
        /// </summary>
        protected IEnumerable<T> CurrentSelection {
            get {
                if (_selection == null)
                    return Enumerable.Empty<T>();
                return _selection.Where(IsSelectionValid);
            }
        }

        /// <summary> 
        /// Gets a SelectionMode filter on the selection's objects. 
        /// </summary>
        protected SelectionMode SelectionMode => SelectionMode.Unfiltered;

        /// <summary> 
        /// Unity event: invoked when the editor selection has changed. 
        /// </summary>
        protected virtual void OnSelectionChange() {
            if (IsLocked)
                return;
            _selection = Selection.GetFiltered(typeof(T), SelectionMode).Cast<T>().ToArray();
        }

        /// <summary>
        /// Checks if an object is valid to include in the CurrentSelection.
        /// </summary>
        /// <param name="selectedObject">the object in the Editor selection to evaluate</param>
        /// <returns> true if the object is valid, false otherwise. </returns>
        protected virtual bool IsSelectionValid(T selectedObject) {
            return true;
        }

    }

}