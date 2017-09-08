using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityKeyCodeComboSource : InputSource {

        public KeyCode[] KeyCodeList { get; set; }

        public UnityKeyCodeComboSource(params KeyCode[] keyCodeList) { 
            KeyCodeList = keyCodeList; 
        }

        public override bool GetState(InputDevice inputDevice) { 
            return KeyCodeList.Any(Input.GetKey); 
        }

    }

}