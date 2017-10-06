#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace XInputDotNetPure {

    internal class Imports {

        internal const string DLLName = "XInputInterface";

        [DllImport(DLLName)]
        public static extern uint XInputGamePadGetState(uint playerIndex, IntPtr state);

        [DllImport(DLLName)]
        public static extern void XInputGamePadSetState(uint playerIndex, float leftMotor, float rightMotor);

    }

    public enum ButtonState {

        Pressed,
        Released

    }

    public struct GamePadButtons {
        public ButtonState Start;
        public ButtonState Back;
        public ButtonState LeftStick;
        public ButtonState RightStick;
        public ButtonState LeftShoulder;
        public ButtonState RightShoulder;
        public ButtonState A;
        public ButtonState B;
        public ButtonState X;
        public ButtonState Y;
    }

    public struct GamePadDPad {
        public ButtonState Up;
        public ButtonState Down;
        public ButtonState Left;
        public ButtonState Right;
    }

    public struct GamePadThumbSticks {
        public Vector2 Left;
        public Vector2 Right;
    }

    public struct GamePadTriggers {
        public float Left;
        public float Right;
    }

    public struct GamePadState {

        internal struct RawState {

            public uint dwPacketNumber;
            public GamePad Gamepad;

            public struct GamePad {

                public ushort dwButtons;
                public byte bLeftTrigger;
                public byte bRightTrigger;
                public short sThumbLX;
                public short sThumbLY;
                public short sThumbRX;
                public short sThumbRY;

            }

        }

        bool isConnected;
        uint packetNumber;
        GamePadButtons buttons;
        GamePadDPad dPad;
        GamePadThumbSticks thumbSticks;
        GamePadTriggers triggers;

        enum ButtonsConstants {

            DPadUp = 0x00000001,
            DPadDown = 0x00000002,
            DPadLeft = 0x00000004,
            DPadRight = 0x00000008,
            Start = 0x00000010,
            Back = 0x00000020,
            LeftThumb = 0x00000040,
            RightThumb = 0x00000080,
            LeftShoulder = 0x0100,
            RightShoulder = 0x0200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000

        }

        internal GamePadState(bool isConnected, RawState rawState, GamePadDeadZone deadZone) {
            this.isConnected = isConnected;

            if (!isConnected) {
                rawState.dwPacketNumber = 0;
                rawState.Gamepad.dwButtons = 0;
                rawState.Gamepad.bLeftTrigger = 0;
                rawState.Gamepad.bRightTrigger = 0;
                rawState.Gamepad.sThumbLX = 0;
                rawState.Gamepad.sThumbLY = 0;
                rawState.Gamepad.sThumbRX = 0;
                rawState.Gamepad.sThumbRY = 0;
            }

            packetNumber = rawState.dwPacketNumber;
            buttons =
                new GamePadButtons {
                    Start = GetState(rawState, ButtonsConstants.Start),
                    Back  = GetState(rawState, ButtonsConstants.Start),
                    LeftStick = GetState(rawState, ButtonsConstants.LeftThumb),
                    RightStick = GetState(rawState, ButtonsConstants.RightThumb),
                    LeftShoulder = GetState(rawState, ButtonsConstants.LeftShoulder),
                    RightShoulder = GetState(rawState, ButtonsConstants.RightShoulder),
                    A = GetState(rawState, ButtonsConstants.A),
                    B = GetState(rawState, ButtonsConstants.B),
                    X = GetState(rawState, ButtonsConstants.X),
                    Y = GetState(rawState, ButtonsConstants.Y),
                };
            dPad =
                new GamePadDPad {
                    Up = GetState(rawState, ButtonsConstants.DPadUp),
                    Down = GetState(rawState, ButtonsConstants.DPadDown),
                    Left = GetState(rawState, ButtonsConstants.DPadLeft),
                    Right = GetState(rawState, ButtonsConstants.DPadRight),
                };

            thumbSticks =
                new GamePadThumbSticks {
                    Left = Utils.ApplyLeftStickDeadZone(rawState.Gamepad.sThumbLX, rawState.Gamepad.sThumbLY, deadZone),
                    Right = Utils.ApplyRightStickDeadZone(rawState.Gamepad.sThumbRX, rawState.Gamepad.sThumbRY, deadZone)
                };
            triggers =new GamePadTriggers {
                Left = Utils.ApplyTriggerDeadZone(rawState.Gamepad.bLeftTrigger, deadZone),
                Right = Utils.ApplyTriggerDeadZone(rawState.Gamepad.bRightTrigger, deadZone)
            };
        }

        static ButtonState GetState(RawState state, ButtonsConstants mask) {
            return (state.Gamepad.dwButtons & (uint) ButtonsConstants.DPadRight) != 0
                        ? ButtonState.Pressed
                        : ButtonState.Released;
        }

        public uint PacketNumber { get { return packetNumber; } }
        public bool IsConnected { get { return isConnected; } }
        public GamePadButtons Buttons { get { return buttons; } }
        public GamePadDPad DPad { get { return dPad; } }
        public GamePadTriggers Triggers { get { return triggers; } }
        public GamePadThumbSticks ThumbSticks { get { return thumbSticks; } }

    }

    public enum PlayerIndex {

        One = 0,
        Two,
        Three,
        Four

    }

    public enum GamePadDeadZone {

        Circular,
        IndependentAxes,
        None

    }

    public class GamePad {

        public static GamePadState GetState(PlayerIndex playerIndex) {
            return GetState(playerIndex, GamePadDeadZone.IndependentAxes);
        }

        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZone) {
            IntPtr gamePadStatePointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GamePadState.RawState)));
            uint result = Imports.XInputGamePadGetState((uint) playerIndex, gamePadStatePointer);
            var state =
                (GamePadState.RawState) Marshal.PtrToStructure(gamePadStatePointer, typeof(GamePadState.RawState));
            return new GamePadState(result == Utils.Success, state, deadZone);
        }

        public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor) {
            Imports.XInputGamePadSetState((uint) playerIndex, leftMotor, rightMotor);
        }

    }

}

#endif