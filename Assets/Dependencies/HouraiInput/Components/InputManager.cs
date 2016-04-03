using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class InputManager : MonoBehaviour {
        public bool logDebugInfo = false;
        public bool invertYAxis = false;
        public bool enableXInput = false;
        public bool useFixedUpdate = false;
        public bool dontDestroyOnLoad = false;
        public List<string> customProfiles = new List<string>();


        private void OnEnable() {
            if (logDebugInfo) {
                Logger.OnLogMessage += HandleOnLogMessage;
            }

            HInput.InvertYAxis = invertYAxis;
            HInput.EnableXInput = enableXInput;
            HInput.SetupInternal();

            foreach (var className in customProfiles) {
                var classType = Type.GetType(className);
                if (classType == null) {
                    Debug.LogError("Cannot find class for custom profile: " + className);
                }
                else {
                    var customProfileInstance = Activator.CreateInstance(classType) as UnityInputDeviceProfile;
                    HInput.AttachDevice(new UnityInputDevice(customProfileInstance));
                }
            }

            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(this);
            }
        }

        private void OnDisable() {
            HInput.ResetInternal();
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
                HInput.UpdateInternal();
            }
        }


        private void FixedUpdate() {
            if (useFixedUpdate) {
                HInput.UpdateInternal();
            }
        }


        private void OnApplicationFocus(bool focusState) {
            HInput.OnApplicationFocus(focusState);
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
