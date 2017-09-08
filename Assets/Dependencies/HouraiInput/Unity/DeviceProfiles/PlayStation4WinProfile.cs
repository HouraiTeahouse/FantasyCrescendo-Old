namespace HouraiTeahouse.HouraiInput {

    // @cond nodoc
    [AutoDiscover]
    public class PlayStation4WinProfile : UnityInputDeviceProfile {

        public PlayStation4WinProfile() {
            Name = "PlayStation 4 Controller";
            Meta = "PlayStation 4 Controller on Windows";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {"Wireless Controller"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "Cross", Target = InputTarget.Action1, Source = Button(1)},
                new InputMapping {Handle = "Circle", Target = InputTarget.Action2, Source = Button(2)},
                new InputMapping {Handle = "Square", Target = InputTarget.Action3, Source = Button(0)},
                new InputMapping {Handle = "Triangle", Target = InputTarget.Action4, Source = Button(3)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(4)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(5)},
                new InputMapping {Handle = "Share", Target = InputTarget.Share, Source = Button(8)},
                new InputMapping {Handle = "Options", Target = InputTarget.Select, Source = Button(9)},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button(10)},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button(11)
                },
                new InputMapping {Handle = "System", Target = InputTarget.System, Source = Button(12)},
                new InputMapping {Handle = "TouchPad Button", Target = InputTarget.TouchPadTap, Source = Button(13)},
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
                    Source = Analog(5),
                    Invert = true
                },
                new InputMapping {
                    Handle = "Left Trigger",
                    Target = InputTarget.LeftTrigger,
                    Source = Analog(3),
                    SourceRange = InputMapping.Complete,
                    TargetRange = InputMapping.Positive,
                    IgnoreInitialZeroValue = true
                },
                new InputMapping {
                    Handle = "Right Trigger",
                    Target = InputTarget.RightTrigger,
                    Source = Analog(4),
                    SourceRange = InputMapping.Complete,
                    TargetRange = InputMapping.Positive,
                    IgnoreInitialZeroValue = true
                },
                new InputMapping {
                    Handle = "DPad Left",
                    Target = InputTarget.DPadLeft,
                    Source = Analog(6),
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {
                    Handle = "DPad Right",
                    Target = InputTarget.DPadRight,
                    Source = Analog(6),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Up",
                    Target = InputTarget.DPadUp,
                    Source = Analog(7),
                    SourceRange = InputMapping.Positive,
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog(7),
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                }
            };
        }

    }

}