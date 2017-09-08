using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> 
    /// Set of extension methods for GameObjects 
    /// </summary>
    public static class GameObjectExtensions {

        /// <summary> 
        /// Gets a component of a certain type. If one doesn't exist, one will be added and returned. 
        /// </summary>
        /// <param name="gameObject"> the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> is null </exception>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
            var attempt = Argument.NotNull(gameObject).GetComponent<T>();
            return attempt ? attempt : gameObject.AddComponent<T>();
        }

        /// <summary> 
        /// Gets a component of a certain type on a GameObject. Works exactly like the normal GetComponent, but also logs
        /// an error in the console if one is not found. 
        /// </summary>
        /// <typeparam name="T"> the type of the component to retrieve </typeparam>
        /// <param name="gameObject"> the GameObject to retrieve the Component </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> is null </exception>
        public static T SafeGetComponent<T>(this GameObject gameObject) where T : class {
            var attempt = Argument.NotNull(gameObject).GetComponent<T>();
            if (attempt == null)
                Log.Warning("Attempted to find a component of type {0} on {1}, but did not find one.", typeof(T), gameObject.name);
            return attempt;
        }

        /// <summary>
        /// Like SafeGetComponent, but operates on the children of the GameObject as well.
        /// </summary>
        /// <param name="component"> the component to base the search on </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        public static T SafeGetComponentInChildren<T>(this GameObject gameObject) where T : class {
            var attempt = Argument.NotNull(gameObject).GetComponentInChildren<T>();
            if (attempt == null)
                Log.Warning("Attempted to find a component of type {0} on child of {1}, but did not find one.", typeof(T), gameObject.name);
            return attempt;
        }

        /// <summary>
        /// Like SafeGetComponent, but operates on the ancestors of the GameObject as well.
        /// </summary>
        /// <param name="component"> the component to base the search on </param>
        /// <returns> the retrieved Component </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="component" /> is null </exception>
        public static T SafeGetComponentInParent<T>(this GameObject gameObject) where T : class {
            var attempt = Argument.NotNull(gameObject).GetComponentInParent<T>();
            if (attempt == null)
                Log.Warning("Attempted to find a component of type {0} on ancestor of {1}, but did not find one.", typeof(T), gameObject.name);
            return attempt;
        }

        public static T Apply<T>(this GameObject gameObject, Action<T> action, bool includeChildren = false) {
            Argument.NotNull(gameObject);
            Argument.NotNull(action);
            var component = includeChildren ? gameObject.GetComponentInChildren<T>() : gameObject.GetComponent<T>();
            if (component != null)
                action(component);
            return component;
        }

        public static T[] ApplyAll<T>(this GameObject gameObject, Action<T> action, bool includeChildren = false) {
            Argument.NotNull(gameObject);
            Argument.NotNull(action);
            var components = includeChildren ? gameObject.GetComponentsInChildren<T>() : gameObject.GetComponents<T>();
            foreach (var component in components) {
                if (component == null)
                    continue;
                try {
                    action(component);
                } catch (Exception e) {
                    Log.Error(e);
                }
            }
            return components;
        }

        /// <summary> 
        /// Gets all Components of a certain type that are attached to a set of GameObjects. 
        /// </summary>
        /// <typeparam name="T"> the type of component to retrieve, can be an interface </typeparam>
        /// <param name="gameObjects"> the GameObjects to retrieve </param>
        /// <returns> an enumeration of all components of the type attached to the GameObjects </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObjects" /> is null </exception>
        public static IEnumerable<T> GetComponents<T>(this IEnumerable<GameObject> gameObjects) where T : class {
            return Argument.NotNull(gameObjects)
                           .IgnoreNulls()
                           .SelectMany(gameObject => gameObject.GetComponents<T>());
        }

        /// <summary> 
        /// Checks if a GameObject's layer is in a LayerMask or not. 
        /// </summary>
        /// <param name="gameObject"> the GameObject to check </param>
        /// <param name="mask"> the layer to check </param>
        /// <returns> whether <paramref name="gameObject" /> fits the layer described </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="gameObject" /> is null </exception>
        public static bool LayerCheck(this GameObject gameObject, LayerMask mask) {
            return ((1 << Argument.NotNull(gameObject).layer) & mask) != 0;
        }

    }

}
