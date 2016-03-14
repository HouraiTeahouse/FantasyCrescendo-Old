using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityInputDeviceManager : InputDeviceManager {
        private const float deviceRefreshInterval = 1.0f;

        private readonly List<UnityInputDeviceProfile> deviceProfiles = new List<UnityInputDeviceProfile>();
        private float deviceRefreshTimer;
        private string joystickHash = "";
        private bool keyboardDevicesAttached;


        public UnityInputDeviceManager() {
            AutoDiscoverDeviceProfiles();
            RefreshDevices();
        }


        private static string JoystickHash {
            get {
                var joystickNames = Input.GetJoystickNames();
                return joystickNames.Length + ": " + string.Join(", ", joystickNames);
            }
        }


        public override void Update(ulong updateTick, float deltaTime) {
            deviceRefreshTimer += deltaTime;
            if (string.IsNullOrEmpty(joystickHash) || deviceRefreshTimer >= deviceRefreshInterval) {
                deviceRefreshTimer = 0.0f;

                if (joystickHash != JoystickHash) {
                    Logger.LogInfo("Change in Unity attached joysticks detected; refreshing device list.");
                    RefreshDevices();
                }
            }
        }


        private void RefreshDevices() {
            AttachKeyboardDevices();
            DetectAttachedJoystickDevices();
            DetectDetachedJoystickDevices();
            joystickHash = JoystickHash;
        }


        private void AttachDevice(UnityInputDevice device) {
            devices.Add(device);
            InputManager.AttachDevice(device);
        }


        private void AttachKeyboardDevices() {
            var deviceProfileCount = deviceProfiles.Count;
            for (var i = 0; i < deviceProfileCount; i++) {
                var deviceProfile = deviceProfiles[i];
                if (deviceProfile.IsNotJoystick && deviceProfile.IsSupportedOnThisPlatform) {
                    AttachKeyboardDeviceWithConfig(deviceProfile);
                }
            }
        }


        private void AttachKeyboardDeviceWithConfig(UnityInputDeviceProfile config) {
            if (keyboardDevicesAttached) {
                return;
            }

            var keyboardDevice = new UnityInputDevice(config);
            AttachDevice(keyboardDevice);

            keyboardDevicesAttached = true;
        }


        private void DetectAttachedJoystickDevices() {
            try {
                var joystickNames = Input.GetJoystickNames();
                for (var i = 0; i < joystickNames.Length; i++) {
                    DetectAttachedJoystickDevice(i + 1, joystickNames[i]);
                }
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

            // PS4 controller works properly as of Unity 4.5
            if (InputManager.UnityVersion <= new VersionInfo(4, 5)) {
                if (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer ||
                    Application.platform == RuntimePlatform.OSXWebPlayer) {
                    if (unityJoystickName == "Unknown Wireless Controller") {
                        // Ignore PS4 controller in Bluetooth mode on Mac since it connects but does nothing.
                        return;
                    }
                }
            }

            // As of Unity 4.6.3p1, empty strings on windows represent disconnected devices.
            if (InputManager.UnityVersion >= new VersionInfo(4, 6, 3)) {
                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.WindowsWebPlayer) {
                    if (string.IsNullOrEmpty(unityJoystickName)) {
                        return;
                    }
                }
            }

            var matchedDeviceProfile = deviceProfiles.Find(config => config.HasJoystickName(unityJoystickName));

            if (matchedDeviceProfile == null) {
                matchedDeviceProfile = deviceProfiles.Find(config => config.HasLastResortRegex(unityJoystickName));
            }

            UnityInputDeviceProfile deviceProfile = null;

            if (matchedDeviceProfile == null) {
                deviceProfile = new UnityUnknownDeviceProfile(unityJoystickName);
                deviceProfiles.Add(deviceProfile);
            }
            else {
                deviceProfile = matchedDeviceProfile;
            }

            var deviceCount = devices.Count;
            for (var i = 0; i < deviceCount; i++) {
                var device = devices[i];
                var unityDevice = device as UnityInputDevice;
                if (unityDevice != null && unityDevice.IsConfiguredWith(deviceProfile, unityJoystickId)) {
                    Logger.LogInfo("Device \"" + unityJoystickName + "\" is already configured with " +
                                   deviceProfile.Name);
                    return;
                }
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
            var joystickNames = Input.GetJoystickNames();

            for (var i = devices.Count - 1; i >= 0; i--) {
                var inputDevice = devices[i] as UnityInputDevice;

                if (inputDevice.Profile.IsNotJoystick) {
                    continue;
                }

                if (joystickNames.Length < inputDevice.JoystickId ||
                    !inputDevice.Profile.HasJoystickOrRegexName(joystickNames[inputDevice.JoystickId - 1])) {
                    devices.Remove(inputDevice);
                    InputManager.DetachDevice(inputDevice);

                    Logger.LogInfo("Detached device: " + inputDevice.Profile.Name);
                }
            }
        }


        private void AutoDiscoverDeviceProfiles() {
            foreach (var typeName in UnityInputDeviceProfileList.Profiles) {
                var deviceProfile = (UnityInputDeviceProfile) Activator.CreateInstance(Type.GetType(typeName));
                if (deviceProfile.IsSupportedOnThisPlatform) {
                    // Logger.LogInfo( "Found profile: " + deviceProfile.GetType().Name + " (" + deviceProfile.Name + ")" );
                    deviceProfiles.Add(deviceProfile);
                }
            }
        }
    }
}