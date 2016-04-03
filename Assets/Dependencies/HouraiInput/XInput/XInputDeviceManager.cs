#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace HouraiTeahouse.HouraiInput {
    public sealed class XInputDeviceManager : InputDeviceManager {
        private readonly bool[] _deviceConnected = {false, false, false, false};

        public XInputDeviceManager() {
            for (var deviceIndex = 0; deviceIndex < 4; deviceIndex++)
                devices.Add(new XInputDevice(deviceIndex));

            Update(0, 0.0f);
        }

        public override void Update(ulong updateTick, float deltaTime) {
            for (var deviceIndex = 0; deviceIndex < 4; deviceIndex++) {
                var device = devices[deviceIndex] as XInputDevice;

                // Unconnected devices won't be updated otherwise, so poll here.
                if (!device.IsConnected) 
                    device.Update(updateTick, deltaTime);

                if (device.IsConnected == _deviceConnected[deviceIndex]) continue;
                if (device.IsConnected)
                    HInput.AttachDevice(device);
                else 
                    HInput.DetachDevice(device);

                _deviceConnected[deviceIndex] = device.IsConnected;
            }
        }

        public static bool CheckPlatformSupport(ICollection<string> errors) {
            if (Application.platform != RuntimePlatform.WindowsPlayer &&
                Application.platform != RuntimePlatform.WindowsEditor) {
                return false;
            }

            try {
                GamePad.GetState(PlayerIndex.One);
            }
            catch (DllNotFoundException e) {
                if (errors != null) {
                    errors.Add(e.Message + ".dll could not be found or is missing a dependency.");
                }
                return false;
            }

            return true;
        }

        public static void Enable() {
            var errors = new List<string>();
            if (CheckPlatformSupport(errors)) {
                HInput.HideDevicesWithProfile(typeof (Xbox360WinProfile));
                HInput.HideDevicesWithProfile(typeof (XboxOneWinProfile));
                HInput.HideDevicesWithProfile(typeof (LogitechF710ModeXWinProfile));
                HInput.HideDevicesWithProfile(typeof (LogitechF310ModeXWinProfile));
                HInput.AddDeviceManager<XInputDeviceManager>();
            }
            else {
                foreach (string error in errors)
                    Logger.LogError(error);
            }
        }
    }
}

#endif
