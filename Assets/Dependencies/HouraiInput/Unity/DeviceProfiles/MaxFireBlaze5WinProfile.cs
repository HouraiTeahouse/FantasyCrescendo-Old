﻿namespace HouraiTeahouse.HouraiInput {

    public class MaxFireBlaze5Profile : UnityInputDeviceProfile {

        public MaxFireBlaze5Profile() {
            Name = "MaxFire Blaze5";
            Meta = "MaxFire Blaze5 Controller on Windows";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {"Controller (MaxFire Blaze5)"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "1", Target = InputTarget.Action1, Source = Button(0)},
                new InputMapping {Handle = "2", Target = InputTarget.Action2, Source = Button(1)},
                new InputMapping {Handle = "3", Target = InputTarget.Action3, Source = Button(2)},
                new InputMapping {Handle = "4", Target = InputTarget.Action4, Source = Button(3)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(4)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(5)},
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button(7)},
                new InputMapping {Handle = "Select", Target = InputTarget.Select, Source = Button(6)},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button(8)},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button(9)
                }
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
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Analog(8),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive,
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
                    Source = Analog(9),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive,
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog(5),
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog(5),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog(6),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive,
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog(6),
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                }
            };
        }

    }

}