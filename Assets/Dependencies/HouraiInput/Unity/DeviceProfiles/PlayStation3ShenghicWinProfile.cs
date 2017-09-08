namespace HouraiTeahouse.HouraiInput {

    // @cond nodoc
    [AutoDiscover]
    public class PlayStation3ShenghicWinProfile : UnityInputDeviceProfile {

        public PlayStation3ShenghicWinProfile() {
            Name = "PlayStation 3 Controller";
            Meta = "Shenghic PlayStation 3 Controller on Windows (not MotionInJoy)";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {
                "PLAYSTATION(R)3Conteroller" // Not a typo.
            };

            ButtonMappings = new[] {
                new InputMapping {Handle = "Cross", Target = InputTarget.Action1, Source = Button(14)},
                new InputMapping {Handle = "Circle", Target = InputTarget.Action2, Source = Button(13)},
                new InputMapping {Handle = "Square", Target = InputTarget.Action3, Source = Button(15)},
                new InputMapping {Handle = "Triangle", Target = InputTarget.Action4, Source = Button(12)},
                new InputMapping {Handle = "DPad Up", Target = InputTarget.DPadUp, Source = Button(4)},
                new InputMapping {Handle = "DPad Down", Target = InputTarget.DPadDown, Source = Button(6)},
                new InputMapping {Handle = "DPad Left", Target = InputTarget.DPadLeft, Source = Button(7)},
                new InputMapping {Handle = "DPad Right", Target = InputTarget.DPadRight, Source = Button(5)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(10)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(11)},
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Button(8)},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Button(9)},
                new InputMapping {Handle = "Select", Target = InputTarget.Select, Source = Button(3)},
                new InputMapping {Handle = "Left Stick Button", Target = InputTarget.LeftStickButton, Source = Button(1)},
                new InputMapping {
                    Handle = "Right Stick Button",
                    Target = InputTarget.RightStickButton,
                    Source = Button(2)
                },
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button(3)},
                new InputMapping {Handle = "System", Target = InputTarget.System, Source = Button(16)}
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
                }
            };
        }

    }

}