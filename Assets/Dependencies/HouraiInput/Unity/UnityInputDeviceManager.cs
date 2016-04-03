using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityInputDeviceManager : InputDeviceManager {

        private float deviceRefreshTimer;
        const float deviceRefreshInterval = 1.0f;

        private readonly List<UnityInputDeviceProfile> _deviceProfiles = new List<UnityInputDeviceProfile>();
        private bool _keyboardDevicesAttached;
        private string _joystickHash = "";

        public UnityInputDeviceManager() {
            AutoDiscoverDeviceProfiles();
            RefreshDevices();
        }

        public override void Update(ulong updateTick, float deltaTime) {
            deviceRefreshTimer += deltaTime;
            if (!string.IsNullOrEmpty(_joystickHash) && !(deviceRefreshTimer >= deviceRefreshInterval)) return;
            deviceRefreshTimer = 0.0f;

            if (_joystickHash == JoystickHash) return;
            Logger.LogInfo("Change in Unity attached joysticks detected; refreshing device list.");
            RefreshDevices();
        }

        private void RefreshDevices() {
            AttachKeyboardDevices();
            DetectAttachedJoystickDevices();
            DetectDetachedJoystickDevices();
            _joystickHash = JoystickHash;
        }

        private void AttachDevice(InputDevice device) {
            devices.Add(device);
            HInput.AttachDevice(device);
        }

        private void AttachKeyboardDevices() {
            foreach (UnityInputDeviceProfile deviceProfile in _deviceProfiles)
                if (!deviceProfile.IsJoystick && deviceProfile.IsSupportedOnThisPlatform)
                    AttachKeyboardDeviceWithConfig(deviceProfile);
        }


        private void AttachKeyboardDeviceWithConfig(UnityInputDeviceProfile config) {
            if (_keyboardDevicesAttached)
                return;

            var keyboardDevice = new UnityInputDevice(config);
            AttachDevice(keyboardDevice);

            _keyboardDevicesAttached = true;
        }


        private void DetectAttachedJoystickDevices() {
            try {
                string[] joystickNames = Input.GetJoystickNames();
                for (var i = 0; i < joystickNames.Length; i++)
                    DetectAttachedJoystickDevice(i + 1, joystickNames[i]);
            }
            catch (Exception e) {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
            }
        }

        private void DetectAttachedJoystickDevice(int unityJoystickId, string unityJoystickName) {
            if (unityJoystickName == "WIRED CONTROLLER" ||
                unityJoystickName == " WIRED CONTROLLER") {
                // Ignore Steam controller for now.
                return;
            }

            if (unityJoystickName.IndexOf("webcam", StringComparison.OrdinalIgnoreCase) != -1) {
                // Unity thinks some webcams are joysticks. >_<
                return;
            }

            // As of Unity 4.6.3p1, empty strings on windows represent disconnected devices.
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.WindowsWebPlayer) {
                if (string.IsNullOrEmpty(unityJoystickName)) {
                    return;
                }
            }

            var matchedDeviceProfile = _deviceProfiles.Find(config => config.HasJoystickName(unityJoystickName));

            if (matchedDeviceProfile == null)
                matchedDeviceProfile = _deviceProfiles.Find(config => config.HasLastResortRegex(unityJoystickName));

            UnityInputDeviceProfile deviceProfile;

            if (matchedDeviceProfile == null) {
                deviceProfile = new UnityUnknownDeviceProfile(unityJoystickName);
                _deviceProfiles.Add(deviceProfile);
            }
            else {
                deviceProfile = matchedDeviceProfile;
            }

            if (devices.OfType<UnityInputDevice>().Any(unityDevice => unityDevice.IsConfiguredWith(deviceProfile, unityJoystickId))) {
                Logger.LogInfo("Device \"" + unityJoystickName + "\" is already configured with " +
                               deviceProfile.Name);
                return;
            }

            if (!deviceProfile.IsHidden) {
                var joystickDevice = new UnityInputDevice(deviceProfile, unityJoystickId);
                AttachDevice(joystickDevice);

                if (matchedDeviceProfile == null) {
                    Logger.LogWarning("Device " + unityJoystickId + " with name \"" + unityJoystickName +
                                      "\" does not match any known profiles.");
                }
                else {
                    Logger.LogInfo("Device " + unityJoystickId + " matched profile " + deviceProfile.GetType().Name +
                                   " (" + deviceProfile.Name + ")");
                }
            }
            else {
                Logger.LogInfo("Device " + unityJoystickId + " matching profile " + deviceProfile.GetType().Name + " (" +
                               deviceProfile.Name + ")" + " is hidden and will not be attached.");
            }
        }


        private void DetectDetachedJoystickDevices() {
            string[] joystickNames = Input.GetJoystickNames();

            for (int i = devices.Count - 1; i >= 0; i--) {
                var inputDevice = devices[i] as UnityInputDevice;

                if (inputDevice == null || !inputDevice.Profile.IsJoystick)
                    continue;

                if (joystickNames.Length >= inputDevice.JoystickId &&
                    inputDevice.Profile.HasJoystickOrRegexName(joystickNames[inputDevice.JoystickId - 1])) continue;
                devices.Remove(inputDevice);
                HInput.DetachDevice(inputDevice);

                Logger.LogInfo("Detached device: " + inputDevice.Profile.Name);
            }
        }


        private void AutoDiscoverDeviceProfiles() {
            foreach (string typeName in UnityInputDeviceProfileList.Profiles) {
                Type type = Type.GetType(typeName);
                if (type == null) continue;
                var deviceProfile = Activator.CreateInstance(type) as UnityInputDeviceProfile;
                if (deviceProfile != null && deviceProfile.IsSupportedOnThisPlatform)
                    _deviceProfiles.Add(deviceProfile);
            }
        }

        static string JoystickHash {
            get {
                string[] joystickNames = Input.GetJoystickNames();
                return string.Format("{0}: {1}", joystickNames.Length, string.Join(", ", joystickNames));
            }
        }
    }
}
