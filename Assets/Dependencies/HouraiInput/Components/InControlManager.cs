using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class InControlManager : MonoBehaviour {
        public bool logDebugInfo = false;
        public bool invertYAxis = false;
        public bool enableXInput = false;
        public bool useFixedUpdate = false;
        public bool dontDestroyOnLoad = false;
        public List<string> customProfiles = new List<string>();


        private void OnEnable() {
            if (logDebugInfo) {
                Debug.Log("InControl (version " + InputManager.Version + ")");
                Logger.OnLogMessage += HandleOnLogMessage;
            }

            InputManager.InvertYAxis = invertYAxis;
            InputManager.EnableXInput = enableXInput;
            InputManager.SetupInternal();

            foreach (var className in customProfiles) {
                var classType = Type.GetType(className);
                if (classType == null) {
                    Debug.LogError("Cannot find class for custom profile: " + className);
                }
                else {
                    var customProfileInstance = Activator.CreateInstance(classType) as UnityInputDeviceProfile;
                    InputManager.AttachDevice(new UnityInputDevice(customProfileInstance));
                }
            }

            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(this);
            }
        }


        private void OnDisable() {
            InputManager.ResetInternal();
        }


#if UNITY_ANDROID && INCONTROL_OUYA && !UNITY_EDITOR
		void Start()
		{
			StartCoroutine( CheckForOuyaEverywhereSupport() );
		}


		IEnumerator CheckForOuyaEverywhereSupport()
		{
			while (!OuyaSDK.isIAPInitComplete())
			{
				yield return null;
			}

			OuyaEverywhereDeviceManager.Enable();
		}
		#endif


        private void Update() {
            if (!useFixedUpdate || Mathf.Approximately(Time.timeScale, 0.0f)) {
                InputManager.UpdateInternal();
            }
        }


        private void FixedUpdate() {
            if (useFixedUpdate) {
                InputManager.UpdateInternal();
            }
        }


        private void OnApplicationFocus(bool focusState) {
            InputManager.OnApplicationFocus(focusState);
        }


        private void OnApplicationPause(bool pauseState) {
            InputManager.OnApplicationPause(pauseState);
        }


        private void OnApplicationQuit() {
            InputManager.OnApplicationQuit();
        }


        private void HandleOnLogMessage(LogMessage logMessage) {
            switch (logMessage.type) {
                case LogMessageType.Info:
                    Debug.Log(logMessage.text);
                    break;
                case LogMessageType.Warning:
                    Debug.LogWarning(logMessage.text);
                    break;
                case LogMessageType.Error:
                    Debug.LogError(logMessage.text);
                    break;
            }
        }
    }
}