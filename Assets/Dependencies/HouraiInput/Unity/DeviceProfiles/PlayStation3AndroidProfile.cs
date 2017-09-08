using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    // @cond nodoc
    [AutoDiscover]
    public class PlayStation3AndroidProfile : UnityInputDeviceProfile {

        public PlayStation3AndroidProfile() {
            Name = "PlayStation 3 Controller";
            Meta = "PlayStation 3 Controller on Android";

            SupportedPlatforms = new[] {"Android"};

            JoystickNames = new[] {
                "", // Yes, really.
                "PLAYSTATION(R)3 Controller",
                "SHENGHIC 2009/0708ZXW-V1Inc. PLAYSTATION(R)3Conteroller",
                // Not a typo.
                "Sony PLAYSTATION(R)3 Controller"
            };

            LastResortRegex = "PLAYSTATION(R)3";

            ButtonMappings = new[] {
                new InputMapping {Handle = "Cross", Target = InputTarget.Action1, Source = Button(2)},
                new InputMapping {Handle = "Circle", Target = InputTarget.Action2, Source = Button(3)},
                new InputMapping {Handle = "Square", Target = InputTarget.Action3, Source = Button(0)},
                new InputMapping {Handle = "Triangle", Target = InputTarget.Action4, Source = Button(1)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(4)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(5)},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button(8)},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button(9)
                },
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button(10)},
                new InputMapping {Handle = "System", Target = InputTarget.System, Source = KeyCodeButton(KeyCode.Menu)}
            };

            AnalogMappings = new[] {
                new InputMapping {Handle = "Left Stick X", Target = InputTarget.LeftStickX, Source = Analog(0)},
                new InputMapping {
                    Handle = "Left Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog(1),
                    Invert = true
                },
                new InputMapping {Handle = "Right Stick X", Target = InputTarget.RightStickX, Source = Analog(2)},
                new InputMapping {
                    Handle = "Right Stick Y",
                    Target = InputTarget.RightStickY,
                    Source = Analog(3),
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog(4),
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog(4),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog(5),
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog(5),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Analog(6)},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Analog(7)}
            };
        }

    }

}