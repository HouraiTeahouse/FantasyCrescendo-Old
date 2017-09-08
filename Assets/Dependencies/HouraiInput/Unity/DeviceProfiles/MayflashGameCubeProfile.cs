namespace HouraiTeahouse.HouraiInput {

    public class MayflashGameCubeWinProfile : UnityInputDeviceProfile {

        public MayflashGameCubeWinProfile() {
            // Gamecube Controller Adapter for PC USB
            Name = "GameCube Controller";
            Meta = "GameCube Controller on Windows";

            SupportedPlatforms = new[] {"Windows"};

            JoystickNames = new[] {"MAYFLASH GameCube Controller Adapter"};

            ButtonMappings = new[] {
                new InputMapping {Handle = "A", Target = InputTarget.Action1, Source = Button(1)},
                new InputMapping {Handle = "B", Target = InputTarget.Action2, Source = Button(0)},
                new InputMapping {Handle = "X", Target = InputTarget.Action2, Source = Button(2)},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button(3)},
                new InputMapping {Handle = "Start", Target = InputTarget.Start, Source = Button(9)},
                new InputMapping {Handle = "Z", Target = InputTarget.RightBumper, Source = Button(7)},
                new InputMapping {Handle = "L", Target = InputTarget.LeftTrigger, Source = Button(4)},
                new InputMapping {Handle = "R", Target = InputTarget.RightTrigger, Source = Button(5)},
                new InputMapping {Handle = "DPad Up", Target = InputTarget.DPadUp, Source = Button(12)},
                new InputMapping {Handle = "DPad Down", Target = InputTarget.DPadDown, Source = Button(14)},
                new InputMapping {Handle = "DPad Left", Target = InputTarget.DPadLeft, Source = Button(15)},
                new InputMapping {Handle = "DPad Right", Target = InputTarget.DPadRight, Source = Button(13)}
            };

            AnalogMappings = new[] {
                new InputMapping {Handle = "Control Stick X", Target = InputTarget.LeftStickX, Source = Analog(0)},
                new InputMapping {
                    Handle = "Control Stick Y",
                    Target = InputTarget.LeftStickY,
                    Source = Analog(1),
                    Invert = true
                },
                new InputMapping {Handle = "C Stick X", Target = InputTarget.RightStickX, Source = Analog(5)},
                new InputMapping {Handle = "C Stick Y", Target = InputTarget.RightStickY, Source = Analog(2)}
            };
        }

    }

    // @endcond
}