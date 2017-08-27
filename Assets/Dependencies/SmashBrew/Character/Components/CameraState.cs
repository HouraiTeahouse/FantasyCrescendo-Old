using HouraiTeahouse.SmashBrew.Stage;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/State/Camera State")]
    public class CameraState : MonoBehaviour {

        static MatchCameraTarget _cameraTarget;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() { 
            if (_cameraTarget == null)
                _cameraTarget = FindObjectOfType<MatchCameraTarget>(); 
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            if(_cameraTarget)
                _cameraTarget.RegisterTarget(transform);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            if(_cameraTarget)
                _cameraTarget.UnregisterTarget(transform);
        }

    }

}

