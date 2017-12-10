using UnityEngine;

namespace HouraiTeahouse {

    public sealed class LogManager : MonoBehaviour {

        [SerializeField]
        LogSettings _settings;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Log.Settings = _settings;
        }

    }

}