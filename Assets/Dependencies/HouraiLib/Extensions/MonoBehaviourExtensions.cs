using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    public static class MonoBehaviourExtensions {

        /// <summary>
        /// Gets a component if the cached version of it is not valid.
        /// Uses SafeGetComponent, so not having a component and no cached value will log a warning.
        /// </summary>
        /// <param name="behaviour"> the MonoBehaviour to check on </param>
        /// <param name="val"> the cached value </param>
        /// <param name="defaultVal"> the default value (Can be used for GetComponentInParent, etc. if need be) </param>
        /// <returns> the cached or retrieved component </returns>
        public static T CachedGetComponent<T>(this MonoBehaviour behaviour, T val, Func<T> defaultVal = null) where T : Component{
            Argument.NotNull(behaviour);
            if (val == null)
                return defaultVal != null ? defaultVal() : behaviour.SafeGetComponent<T>();
            return val;
        }

    }

}
