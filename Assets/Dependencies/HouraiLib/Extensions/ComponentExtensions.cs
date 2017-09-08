using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> 
    /// A set of extention methods for all components. 
    /// </summary>
    public static class ComponentExtensions {

        /// <summary> 
        /// Gets the GameObjects of all components. All 
        /// </summary>
        /// <typeparam name="T"> the type of component </typeparam>
        /// <param name="components"> </param>
        /// <returns> a </returns>
        public static IEnumerable<GameObject> GetGameObject<T>(this IEnumerable<T> components) where T : Component {
            return from component in components.IgnoreNulls() 
                   select component.gameObject;
        }

        /// <summary> 
        /// Gets a component of a certain type. If one doesn't exist, one will be added and returned. 
        /// </summary>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="component"> the Component attached to the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        public static T GetOrAddComponent<T>(this Component component) where T : Component {
            return Argument.NotNull(component).gameObject.GetOrAddComponent<T>();
        }

        /// <summary> 
        /// Gets a component of a certain type on a GameObject. Works exactly like the normal GetComponent, but also logs
        /// an error in the console if one is not found. 
        /// </summary>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="component"> the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        public static T SafeGetComponent<T>(this Component component) where T : class {
            return Argument.NotNull(component).gameObject.SafeGetComponent<T>();
        }

        /// <summary>
        /// Like SafeGetComponent, but operates on the children of the component's GameObject as well.
        /// </summary>
        /// <param name="component"> the component to base the search on </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        public static T SafeGetComponentInChildren<T>(this Component component) where T : class {
            return Argument.NotNull(component).gameObject.SafeGetComponentInChildren<T>();
        }

        /// <summary>
        /// Like SafeGetComponent, but operates on the ancestors of the component's GameObject as well.
        /// </summary>
        /// <param name="component"> the component to base the search on </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        public static T SafeGetComponentInParent<T>(this Component component) where T : class {
            return Argument.NotNull(component).gameObject.SafeGetComponentInParent<T>();
        }

    }

}
