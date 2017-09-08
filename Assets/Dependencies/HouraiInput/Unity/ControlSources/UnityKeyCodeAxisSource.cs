using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityKeyCodeAxisSource : InputSource {

        public KeyCode NegativeKeyCode { get; set; }
        public KeyCode PositiveKeyCode { get; set; }

        public UnityKeyCodeAxisSource(KeyCode negativeKeyCode, KeyCode positiveKeyCode) {
            NegativeKeyCode = negativeKeyCode;
            PositiveKeyCode = positiveKeyCode;
        }

        public override float GetValue(InputDevice inputDevice) {
            var axisValue = 0;
            if (Input.GetKey(NegativeKeyCode))
                axisValue--;
            if (Input.GetKey(PositiveKeyCode))
                axisValue++;
            return axisValue;
        }

    }

}