namespace HouraiTeahouse.HouraiInput {

    public class SteelSeriesFreeLinuxProfile : UnityInputDeviceProfile {

        public SteelSeriesFreeLinuxProfile() {
            Name = "SteelSeries Free";
            Meta = "SteelSeries Free on Linux";

            SupportedPlatforms = new[] {"Linux",};

            JoystickNames = new[] {"Zeemote: SteelSeries FREE"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "4", Target = InputTarget.Action1, Source = Button(0)},
                new InputMapping {Handle = "3", Target = InputTarget.Action2, Source = Button(1)},
                new InputMapping {Handle = "1", Target = InputTarget.Action3, Source = Button(3)},
                new InputMapping {Handle = "2", Target = InputTarget.Action4, Source = Button(4)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(6)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(7)},
                new InputMapping {Handle = "Back", Target = InputTarget.Select, Source = Button(12)},
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button(11)}
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
                    TargetRange = InputMapping.Positive,
                    Invert = false
                }
            };
        }

    }

}