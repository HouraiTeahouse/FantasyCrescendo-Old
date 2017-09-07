using HouraiTeahouse.SmashBrew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    /// <summary>
    /// Hides or shows the GameObject that this script is attached to when the game is
    /// paused.
    /// </summary>
    public class PauseVisibility : MonoBehaviour {

        [SerializeField]
        [Tooltip("If true, the object will only be active while paused. Otherwise it will be active only when unpaused.")]
        bool _show;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            SmashTimeManager.OnPause += OnPause;
            OnPause();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            SmashTimeManager.OnPause -= OnPause;
        }

        void OnPause() {
            gameObject.SetActive(!_show ^ SmashTimeManager.Paused);
        }

    }

}