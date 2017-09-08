namespace HouraiTeahouse.HouraiInput {

    public class AppleMFiProfile : UnityInputDeviceProfile {

        public AppleMFiProfile() {
            Name = "Apple MFi Controller";
            Meta = "Apple MFi Controller on iOS";

            SupportedPlatforms = new[] {"iPhone"};

            LastResortRegex = ""; // Match anything.

            LowerDeadZone = 0.05f;
            UpperDeadZone = 0.95f;

            ButtonMappings = new[] {
                new InputMapping {Handle = "A", Target = InputTarget.Action1, Source = Button(14)},
                new InputMapping {Handle = "B", Target = InputTarget.Action2, Source = Button(13)},
                new InputMapping {Handle = "X", Target = InputTarget.Action3, Source = Button(15)},
                new InputMapping {Handle = "Y", Target = InputTarget.Action4, Source = Button(12)},
                new InputMapping {Handle = "DPad Up", Target = InputTarget.DPadUp, Source = Button(4)},
                new InputMapping {Handle = "DPad Down", Target = InputTarget.DPadDown, Source = Button(6)},
                new InputMapping {Handle = "DPad Left", Target = InputTarget.DPadLeft, Source = Button(7)},
                new InputMapping {Handle = "DPad Right", Target = InputTarget.DPadRight, Source = Button(5)},
                new InputMapping {Handle = "Left Bumper", Target = InputTarget.LeftBumper, Source = Button(8)},
                new InputMapping {Handle = "Right Bumper", Target = InputTarget.RightBumper, Source = Button(9)},
                new InputMapping {Handle = "Pause", Target = InputTarget.Pause, Source = Button(0)},
                new InputMapping {Handle = "Left Trigger", Target = InputTarget.LeftTrigger, Source = Button(10)},
                new InputMapping {Handle = "Right Trigger", Target = InputTarget.RightTrigger, Source = Button(11)}
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