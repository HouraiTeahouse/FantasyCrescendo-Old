using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour {
        private static readonly object _lock = new object();
        public static T Instance { get; private set; }


        protected void SetSingletonInstance() {
            lock (_lock) {
                if (Instance == null) {
                    var instances = FindObjectsOfType<T>();
                    if (instances.Length > 0) {
                        Instance = instances[0];

                        if (instances.Length > 1) {
                            Debug.LogWarning("Multiple instances of singleton " + typeof (T) + " found.");
                        }
                    }
                    else {
                        Debug.LogError("No instance of singleton " + typeof (T) + " found.");
                    }
                }
            }
        }
    }
}