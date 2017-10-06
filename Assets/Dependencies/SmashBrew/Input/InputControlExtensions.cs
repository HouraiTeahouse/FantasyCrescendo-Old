using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.HouraiInput;

namespace HouraiTeahouse.SmashBrew {

    public static class InputControlExtensions {

        /// <summary>
        /// Gets all controls
        /// </summary>
        /// <param name="device"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static IEnumerable<InputControl> GetControls(this InputDevice device, 
                                                            IEnumerable<InputTarget> targets) {
            return targets.Select(device.GetControl);
        }

    }

}