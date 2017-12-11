using UnityEngine;

namespace HouraiTeahouse {

    /// <summary>
    /// A UI Menu. Represents a single UI entity where the display is mutually exclusive wtih all other Menus:
    /// only one Menu can be visible at a time.
    /// Managed by the <see cref="MenuManager"/> singleton.
    /// </summary>
    public class Menu : MonoBehaviour {

        [SerializeField]
        [Tooltip("The name and identifier of the Menu. Must be globally unique.")]
        string _name;

        /// <summary>
        /// The name of the Menu. Must be globally unique.
        /// </summary>
        /// <returns></returns>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            var manager = MenuManager.Instance;
            if (manager == null) {
                Debug.LogErrorFormat("No MenuManager available for {0} to register itself onto.", name);
                return;
            }
            manager.Register(this);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            var manager = MenuManager.Instance;
            if (manager == null)
                return;
            manager.Unregister(this);
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() {
            _name = name; 
        }

    }

}