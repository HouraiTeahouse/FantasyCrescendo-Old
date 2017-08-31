using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class InputSlice : MessageBase {

        Vector2b movement;
        byte buttons;

        const float kSmashThreshold = 0.3f;

        public Vector2 Movement {
            get { return movement; }
            set { movement = value; }
        }

        public Vector2 Smash {
            get { 
                return new Vector2 {
                    x = BitsToFloat(4),
                    y = BitsToFloat(6)
                };
            }
            set {
                FloatToBits(4, value.x);
                FloatToBits(6, value.y);
            }
        }

        public bool Attack {
            get { return GetBit(0); }
            set { SetBit(0, value); } 
        }

        public bool Special {
            get { return GetBit(1); }
            set { SetBit(1, value); }
        }

        public bool Jump {
            get { return GetBit(2); }
            set { SetBit(2, value); }
        }

        public bool Shield {
            get { return GetBit(3); }
            set { SetBit(3, value); }
        }

        bool GetBit(int bit) {
            return (buttons & (1 << bit)) != 0;
        }

        void SetBit(int bit, bool val) {
            var bitmask = (byte)(1 << bit);
            if (val)
                buttons |= bitmask;
            else
                buttons &= (byte)~bitmask;
        }

        // Converts two bits from $buttons according to the following mapping:
        //  00 => 0.0
        //  01 => 1.0
        //  10 => -1.0
        //  11 => 0.0
        // $bitShift determies the base position for the two bits.
        float BitsToFloat(int bitShift) {
            var val = (buttons >> bitShift) & 0x3;
            float floatVal = (float)(val & 0x1);
            floatVal -= (float)((val >> 1) & 0x1);
            return floatVal;
        }

        void FloatToBits(int bitShift, float val) {
            byte bVal = 0;
            if (val > kSmashThreshold)
                bVal = 0x1;
            else if(val < -kSmashThreshold)
                bVal = 0x2;
            buttons &= (byte)~(0x3b << bitShift);
            buttons |= (byte)(bVal << bitShift);
        }

        public override void Serialize(NetworkWriter writer) {
            writer.Write(movement.byteX);
            writer.Write(movement.byteY);
            writer.Write(buttons);
        }

        public override void Deserialize(NetworkReader reader) {
            movement = new Vector2b  {
                X = reader.ReadByte(),
                Y = reader.ReadByte()
            };
            buttons = reader.ReadByte();
        }

        public InputSlice Clone() {
            return (InputSlice) MemberwiseClone();
        }

    }

}

