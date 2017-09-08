namespace HouraiTeahouse.HouraiInput {

    public class LogitechWingManWinProfile : UnityInputDeviceProfile {

        public LogitechWingManWinProfile() {
            Name = "Logitech WingMan Controller";
            Meta = "Logitech WingMan Controller on Windows";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {"WingMan Cordless Gamepad",};

            ButtonMappings = new[] {
                new InputMapping {Handle = "A", Target = InputTarget.Action1, Source = Button(1)},
                new InputMapping {Handle = "B", Target = InputTarget.Action2, Source = Button(2)},
                new InputMapping {Handle = "C", Target = InputTarget.Button0, Source = Button(2)},
                new InputMapping {Handle = "X", Target = InputTarget.Action3, Source = Button(4)},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button(5)},
                new InputMapping {Handle = "Z", Target = InputTarget.Button1, Source = Button(6)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(7)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(8)},
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Button(10)},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Button(11)},
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button(9)}
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
                new InputMapping {Handle = "Throttle", Target = InputTarget.Analog0, Source = Analog(2)}
            };
        }

    }

}