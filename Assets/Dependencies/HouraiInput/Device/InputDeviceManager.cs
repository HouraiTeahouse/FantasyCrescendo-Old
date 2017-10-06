using System.Collections.Generic;

namespace HouraiTeahouse.HouraiInput {

    /// <summary>
    /// The abstract base class for all supported DeviceManagers.
    /// Each DeviceManager controls and updates a set of potential input devices.
    /// </summary>
    public abstract class InputDeviceManager {

        protected List<InputDevice> devices = new List<InputDevice>();

        public abstract void Update(ulong updateTick, float deltaTime); 

    }

}