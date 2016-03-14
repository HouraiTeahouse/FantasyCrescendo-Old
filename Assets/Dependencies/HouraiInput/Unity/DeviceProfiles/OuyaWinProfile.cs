using System;


namespace HouraiTeahouse.HouraiInput {
    // @cond nodoc
    [AutoDiscover]
    public class OuyaWinProfile : UnityInputDeviceProfile {
        public OuyaWinProfile() {
            Name = "OUYA Controller";
            Meta = "OUYA Controller on Windows";

            SupportedPlatforms = new[] {
                "Windows"
            };

            JoystickNames = new[] {
                "OUYA Game Controller"
            };

            LowerDeadZone = 0.3f;

            ButtonMappings = new[] {
                new InputControlMapping {
                    Handle = "O",
                    Target = InputControlTarget.Action1,
                    Source = Button0
                },
                new InputControlMapping {
                    Handle = "A",
                    Target = InputControlTarget.Action2,
                    Source = Button3
                },
                new InputControlMapping {
                    Handle = "U",
                    Target = InputControlTarget.Action3,
                    Source = Button1
                },
                new InputControlMapping {
                    Handle = "Y",
                    Target = InputControlTarget.Action4,
                    Source = Button2
                },
                new InputControlMapping {
                    Handle = "Left Bumper",
                    Target = InputControlTarget.LeftBumper,
                    Source = Button4
                },
                new InputControlMapping {
                    Handle = "Right Bumper",
                    Target = InputControlTarget.RightBumper,
                    Source = Button5
                },
                new InputControlMapping {
                    Handle = "Left Stick Button",
                    Target = InputControlTarget.LeftStickButton,
                    Source = Button6
                },
                new InputControlMapping {
                    Handle = "Right Stick Button",
                    Target = InputControlTarget.RightStickButton,
                    Source = Button7
                },
                new InputControlMapping {
                    Handle = "DPad Up",
                    Target = InputControlTarget.DPadUp,
                    Source = Button8
                },
                new InputControlMapping {
                    Handle = "DPad Down",
                    Target = InputControlTarget.DPadDown,
                    Source = Button9
                },
                new InputControlMapping {
                    Handle = "DPad Left",
                    Target = InputControlTarget.DPadLeft,
                    Source = Button10
                },
                new InputControlMapping {
                    Handle = "DPad Right",
                    Target = InputControlTarget.DPadRight,
                    Source = Button11
                },
                new InputControlMapping {
                    Handle = "System",
                    Target = InputControlTarget.System,
                    Source = Button14
                },
                new InputControlMapping {
                    Handle = "TouchPad Tap",
                    Target = InputControlTarget.TouchPadTap,
                    Source = MouseButton0
                }
            };

            AnalogMappings = new[] {
                new InputControlMapping {
                    Handle = "Left Stick X",
                    Target = InputControlTarget.LeftStickX,
                    Source = Analog0
                },
                new InputControlMapping {
                    Handle = "Left Stick Y",
                    Target = InputControlTarget.LeftStickY,
                    Source = Analog1,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "Right Stick X",
                    Target = InputControlTarget.RightStickX,
                    Source = Analog3
                },
                new InputControlMapping {
                    Handle = "Right Stick Y",
                    Target = InputControlTarget.RightStickY,
                    Source = Analog4,
                    Invert = true
                },
                new InputControlMapping {
                    Handle = "Left Trigger",
                    Target = InputControlTarget.LeftTrigger,
                    Source = Analog2,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive
                },
                new InputControlMapping {
                    Handle = "Right Trigger",
                    Target = InputControlTarget.RightTrigger,
                    Source = Analog5,
                    SourceRange = InputControlMapping.Range.Positive,
                    TargetRange = InputControlMapping.Range.Positive
                },
                new InputControlMapping {
                    Handle = "TouchPad X Axis",
                    Target = InputControlTarget.TouchPadXAxis,
                    Source = MouseXAxis,
                    Raw = true
                },
                new InputControlMapping {
                    Handle = "TouchPad Y Axis",
                    Target = InputControlTarget.TouchPadYAxis,
                    Source = MouseYAxis,
                    Raw = true
                }
            };
        }
    }
}
