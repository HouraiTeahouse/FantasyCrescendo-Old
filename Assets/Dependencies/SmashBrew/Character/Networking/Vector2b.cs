using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    public struct Vector2b {

        const float kMaxValue = 127f;

        public sbyte byteX;
        public sbyte byteY;

        public float X {
            get { return byteX / kMaxValue; }
            set { byteX = (sbyte)Mathf.Round(Clamp(value) * kMaxValue); }
        }

        public float Y {
            get { return byteY / kMaxValue; }
            set { byteY = (sbyte)Mathf.Round(Clamp(value) * kMaxValue); }
        }

        float Clamp(float x) {
            return Mathf.Clamp(x, -1f, 1f);
        }

        public static implicit operator Vector2b(Vector2 vector) {
            return new Vector2b {
                X = vector.x,
                Y = vector.y
            };
        }

        public static implicit operator Vector2(Vector2b vector) {
            return new Vector2 {
                x = vector.X,
                y = vector.Y
            };
        }

    }

}
