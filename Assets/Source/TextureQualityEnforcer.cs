using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    /// <summary>
    /// A script that enforces a certain global texture quality level while active.
    /// </summary>
    public class TextureQualityEnforcer : MonoBehaviour {

        int _cachedLevel;

        [SerializeField]
        int _level;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            _cachedLevel = QualitySettings.masterTextureLimit;
            QualitySettings.masterTextureLimit = _level;
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() { 
            QualitySettings.masterTextureLimit = _cachedLevel; 
        }

    }

}
