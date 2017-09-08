using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    /// <summary>
    /// The singleton for HouraiInput. Hooks the rest of HouraiInput into the Unity engine.
    /// </summary>
    public class InputManager : MonoBehaviour {

        static ILog _log = Log.GetLogger<InputManager>();

        [SerializeField]
        [Tooltip("Inverts the Y Axis if true.")]
        bool _invertYAxis = false;

        [SerializeField]
        [Tooltip("Use XInput if true.")]
        bool _enableXInput = false;

        [SerializeField]
        [Tooltip("Updates using FixedUpdate if set to true. Uses Update otherwise.")]
        bool _useFixedUpdate = false;

        [SerializeField]
        [Tooltip("")]
        bool _dontDestroyOnLoad = false;

        [SerializeField]
        List<string> _customProfiles = new List<string>();

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            HInput.InvertYAxis = _invertYAxis;
            HInput.EnableXInput = _enableXInput;
            HInput.SetupInternal();

            foreach (string className in _customProfiles) {
                Type classType = Type.GetType(className);
                if (classType == null)
                    _log.Error("Cannot find class for custom profile: {0}", className);
                else {
                    var customProfileInstance = Activator.CreateInstance(classType) as UnityInputDeviceProfile;
                    HInput.AttachDevice(new UnityInputDevice(customProfileInstance));
                }
            }

            foreach(var device in HInput.Devices)
                _log.Info("Found Device: {0}", device.Name);

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() { 
            HInput.ResetInternal(); 
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (!_useFixedUpdate || Mathf.Approximately(Time.timeScale, 0.0f))
                HInput.UpdateInternal();
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate() {
            if (_useFixedUpdate || Mathf.Approximately(Time.timeScale, 0.0f))
                HInput.UpdateInternal();
        }

        /// <summary>
        /// Callback sent to all game objects when the player gets or loses focus.
        /// </summary>
        /// <param name="focusStatus">The focus state of the application.</param>
        void OnApplicationFocus(bool focusState) { 
            HInput.OnApplicationFocus(focusState); 
        }

    }

}