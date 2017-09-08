using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityMouseButtonSource : InputSource {

        readonly int _buttonId;

        public UnityMouseButtonSource(int buttonId) { 
            _buttonId = buttonId; 
        }

        public override bool GetState(InputDevice inputDevice) { 
            return Input.GetMouseButton(_buttonId); 
        }

    }

}