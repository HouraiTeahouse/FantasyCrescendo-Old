using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class OuyaLinuxProfile : UnityInputDeviceProfile {

        public OuyaLinuxProfile() {
            Name = "OUYA Controller";
            Meta = "OUYA Controller on Linux";

            SupportedPlatforms = new[] {"Linux"};

            JoystickNames = new[] {"OUYA Game Controller"};

            LowerDeadZone = 0.3f;

            ButtonMappings = new[] {
                new InputMapping {Handle = "O", Target = InputTarget.Action1, Source = Button(0)},
                new InputMapping {Handle = "A", Target = InputTarget.Action2, Source = Button(3)},
                new InputMapping {Handle = "U", Target = InputTarget.Action3, Source = Button(1)},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button(2)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(4)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(5)},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button(6)},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button(7)
                },
                new InputMapping {Handle = "System", Target = InputTarget.System, Source = KeyCodeButton(KeyCode.Menu)},
                new InputMapping {Handle = "TouchPad Tap", Target = InputTarget.TouchPadTap, Source = MouseButton(0)},
                new InputMapping {Handle = "DPad Left", Target = InputTarget.DPadLeft, Source = Button(10)},
                new InputMapping {Handle = "DPad Right", Target = InputTarget.DPadRight, Source = Button(11)},
                new InputMapping {Handle = "DPad Up", Target = InputTarget.DPadUp, Source = Button(8)},
                new InputMapping {Handle = "DPad Down", Target = InputTarget.DPadDown, Source = Button(9)},
            };

            AnalogMappings = new[] {
                new InputMapping {Handle = "Left Stick X", Target = InputTarget.LeftStickX, Source = Analog(0)},
                new InputMapping {
                    Handle = "Left Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog(1),
                    Invert = true
                },
                new InputMapping {Handle = "Right Stick X", Target = InputTarget.RightStickX, Source = Analog(3)},
                new InputMapping {
                    Handle = "Right Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog(4),
                    Invert = true
                },
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Analog(2)},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Analog(5)},
                new InputMapping {
                    Handle = "TouchPad X Axis",
                    Target = InputTarget.TouchPadXAxis,
                    Source = MouseXAxis,
                    Raw = true
                },
                new InputMapping {
                    Handle = "TouchPad Y Axis",
                    Target = InputTarget.TouchPadYAxis,
                    Source = MouseYAxis,
                    Raw = true
                }
            };
        }

    }

}