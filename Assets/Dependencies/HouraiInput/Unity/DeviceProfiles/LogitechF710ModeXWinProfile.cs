namespace HouraiTeahouse.HouraiInput {

    public class LogitechF710ModeXWinProfile : UnityInputDeviceProfile {

        public LogitechF710ModeXWinProfile() {
            Name = "Logitech F710 Controller";
            Meta = "Logitech F710 on Windows (XInput Mode)";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {"Controller (Wireless Gamepad F710)"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "A", Target = InputTarget.Action1, Source = Button(0)},
                new InputMapping {Handle = "B", Target = InputTarget.Action2, Source = Button(1)},
                new InputMapping {Handle = "X", Target = InputTarget.Action3, Source = Button(2)},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button(3)},
                new InputMapping {Handle = "Back", Target = InputTarget.Back, Source = Button(6)},
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button(7)},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button(8)},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button(9)
                },
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(4)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(5)}
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
                    TargetRange = InputMapping.Positive
                },
                new InputMapping {
                    Handle = "DPad Down",
                    Target = InputTarget.DPadDown,
                    Source = Analog(6),
                    SourceRange = InputMapping.Negative,
                    TargetRange = InputMapping.Negative,
                    Invert = true
                },
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Analog(8)},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Analog(9)}
            };
        }

    }

}