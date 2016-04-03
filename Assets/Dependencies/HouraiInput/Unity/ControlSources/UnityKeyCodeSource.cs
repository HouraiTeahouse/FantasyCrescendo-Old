using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityKeyCodeSource : InputSource {
        private readonly KeyCode[] _keyCodeList;

        public UnityKeyCodeSource(params KeyCode[] keyCodeList) {
            this._keyCodeList = keyCodeList;
        }


        public float GetValue(InputDevice inputDevice) {
            return GetState(inputDevice) ? 1.0f : 0.0f;
        }


        public bool GetState(InputDevice inputDevice) {
            for (var i = 0; i < _keyCodeList.Length; i++) {
                if (Input.GetKey(_keyCodeList[i])) {
                    return true;
                }
            }
            return false;
        }
    }
}
