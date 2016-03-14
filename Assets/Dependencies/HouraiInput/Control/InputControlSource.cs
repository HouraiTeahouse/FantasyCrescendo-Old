using System;


namespace HouraiTeahouse.HouraiInput {
    public interface InputControlSource {
        float GetValue(InputDevice inputDevice);
        bool GetState(InputDevice inputDevice);
    }
}
