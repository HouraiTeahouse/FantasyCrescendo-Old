using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityKeyCodeSource : InputSource {

        readonly KeyCode _keyCode;

        public UnityKeyCodeSource(KeyCode keycode = KeyCode.None) { 
            _keyCode = keycode; 
        }

        public override bool GetState(InputDevice inputDevice) { 
            return Input.GetKey(_keyCode); 
        }

    }

}