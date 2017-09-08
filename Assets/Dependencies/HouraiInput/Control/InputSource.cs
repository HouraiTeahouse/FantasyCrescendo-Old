using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public abstract class InputSource {

        public virtual float GetValue(InputDevice inputDevice) {
            return GetState(inputDevice) ? 1.0f : 0.0f;
        }

        public virtual bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }

    }

}