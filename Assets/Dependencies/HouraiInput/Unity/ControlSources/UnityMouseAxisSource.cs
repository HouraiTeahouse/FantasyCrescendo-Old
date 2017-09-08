using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityMouseAxisSource : InputSource {

        readonly string _mouseAxisQuery;

        public UnityMouseAxisSource(string axis) { 
            _mouseAxisQuery = "mouse " + axis; 
        }

        public override float GetValue(InputDevice inputDevice) { 
            return Input.GetAxisRaw(_mouseAxisQuery); 
        }

    }

}