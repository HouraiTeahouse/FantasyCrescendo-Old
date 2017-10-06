using UnityEngine;
using UnityEditor;

namespace HouraiTeahouse.Editor {

    /// <summary>
    /// An abstract base class for editors of objects.
    /// </summary>
    public abstract class BaseEditor<T> : ScriptlessEditor where T : Object {

        protected T Target {
            get { return target as T; }
        }

    }

}