#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using UnityEngine;

namespace XInputDotNetPure {

    internal static class Utils {

        public const uint Success = 0x000;
        public const uint NotConnected = 0x000;

        const int LeftStickDeadZone = 7849;
        const int RightStickDeadZone = 8689;
        const int TriggerDeadZone = 30;

        public static float ApplyTriggerDeadZone(byte value, GamePadDeadZone deadZoneMode) {
            if (deadZoneMode == GamePadDeadZone.None) {
                return ApplyDeadZone(value, byte.MaxValue, 0.0f);
            }
            else {
                return ApplyDeadZone(value, byte.MaxValue, TriggerDeadZone);
            }
        }

        public static Vector2 ApplyLeftStickDeadZone(short valueX,
                                                     short valueY,
                                                     GamePadDeadZone deadZoneMode) {
            return ApplyStickDeadZone(valueX, valueY, deadZoneMode, LeftStickDeadZone);
        }

        public static Vector2 ApplyRightStickDeadZone(short valueX,
                                                      short valueY,
                                                      GamePadDeadZone deadZoneMode) {
            return ApplyStickDeadZone(valueX, valueY, deadZoneMode, RightStickDeadZone);
        }

        static Vector2 ApplyStickDeadZone(short valueX,
                                          short valueY,
                                          GamePadDeadZone deadZoneMode,
                                          int deadZoneSize) {
            if (deadZoneMode == GamePadDeadZone.Circular) {
                // Cast to long to avoid int overflow if valueX and valueY are both 32768, which would result in a negative number and Sqrt returns NaN
                var distanceFromCenter =
                    (float) Math.Sqrt((long) valueX * (long) valueX + (long) valueY * (long) valueY);
                float coefficient = ApplyDeadZone(distanceFromCenter, short.MaxValue, deadZoneSize);
                coefficient = coefficient > 0.0f ? coefficient / distanceFromCenter : 0.0f;
                return new Vector2(Clamp(valueX * coefficient), Clamp(valueY * coefficient));
            }
            deadZoneSize = (deadZoneMode == GamePadDeadZone.IndependentAxes) ? deadZoneSize : 0;
            return new Vector2(ApplyDeadZone(valueX, short.MaxValue, deadZoneSize),
                               ApplyDeadZone(valueY, short.MaxValue, deadZoneSize));
        }

        static float Clamp(float value) { return value < -1.0f ? -1.0f : (value > 1.0f ? 1.0f : value); }

        static float ApplyDeadZone(float value, float maxValue, float deadZoneSize) {
            if (value < -deadZoneSize) {
                value += deadZoneSize;
            } else if (value > deadZoneSize) {
                value -= deadZoneSize;
            } else {
                return 0.0f;
            }
            value /= maxValue - deadZoneSize;
            return Clamp(value);
        }

    }

}

#endif