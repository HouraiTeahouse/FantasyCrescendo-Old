using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse {

    /// <summary> 
    /// Component that marks a unique object. Objects instantiated with this attached only allows one to exist.
    /// Trying to create/instantiate more copies will have the object destroyed instantly. 
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UniqueObject : MonoBehaviour, IUniqueEntity<int> {

        /// <summary> 
        /// A collection of all of the UniqueObjects currently in the game. 
        /// </summary>
        static Dictionary<int, UniqueObject> _allIds;

        [SerializeField]
        [ReadOnly]
        [Tooltip("The unique id for this object")]
        int _id;

        /// <summary> 
        /// The unique ID of the object. 
        /// </summary>
        public int ID {
            get { return _id;}
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (_allIds == null)
                _allIds = new Dictionary<int, UniqueObject>();
            UniqueObject obj;
            if (_allIds.TryGetValue(ID, out obj)) {
                // Destroy only destroys the object after a frame is finished, which still allows
                // other code in other attached scripts to execute.
                // DestroyImmediate ensures that said code is not executed and immediately removes the
                // GameObject from the scene.
                Debug.LogWarningFormat("{0} (ID: {1}) already exists. Destroying {2}", obj.name, ID, name);
                DestroyImmediate(gameObject);
                return;
            }
            _allIds[ID] = this;
            Debug.LogFormat("Registered {0} as a unique object. (ID: {1})", name, ID);
        }
        
        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            if (_allIds == null || _allIds[ID] != this)
                return;
            _allIds.Remove(ID);
            if (_allIds.Count <= 0)
                _allIds = null;
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() { 
            _id = new Random().Next(); 
        }

    }

}
