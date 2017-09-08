using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse {

    /// <summary>
    /// A set of static methods, properties, and extension methods to simplify working with
    /// large sets of reflection data.
    /// </summary>
    public static class ReflectionUtilty {

        /// <summary>
        /// Gets an enumeration of all loaded types. Note: this can be a very large enumeration: 
        /// includes anonymous types.
        /// </summary>
        public static IEnumerable<Type> AllTypes {
            get {
                return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                       from type in assembly.GetTypes()
                       select type;
            }
        }

        /// <summary>
        /// Filters an existing set of types to only those that contain classes assignable from a given type.
        /// </summary>
        /// <param name="types"> a provided enumeration of types </param>
        /// <param name="baseType"> the base type to check for </param>
        /// <returns> the filtered enumeration of types </returns>
        /// <exception cref="System.ArgumentNullException"/> 
        ///     <paramref name="types"/> or <paramref name="baseType"/> is null 
        /// </exception>
        public static IEnumerable<Type> IsAssignableFrom(this IEnumerable<Type> types, 
                                                         Type baseType) {
            return Argument.NotNull(types).Where(Argument.NotNull(baseType).IsAssignableFrom);
        }

        /// <summary>
        /// Filterx an existing set of types to only the ones that contain concrete classes.
        /// </summary>
        /// <param name="types"> a provided enumeration of types </param>
        /// <returns> the filtered enumeration of types </returns>
        /// <exception cref="System.ArgumentNullException"/> <paramref name="types"/> is null </exception>
        public static IEnumerable<Type> ConcreteClasses(this IEnumerable<Type> types) {
            return Argument.NotNull(types).Where(t => !t.IsAbstract && t.IsClass);
        }

        /// <summary>
        /// Filterx an existing set of types to only the ones that contain concrete classes.
        /// </summary>
        /// <param name="types"> a provided enumeration of types </param>
        /// <param name="inherit"> whether attributes in base types can be counted or not</param>
        /// <returns> the filtered enumeration of types </returns>
        /// <exception cref="System.ArgumentNullException"/> <paramref name="types"/> is null </exception>
        public static IEnumerable<KeyValuePair<Type, T>> WithAttribute<T>(this IEnumerable<Type> types,
                                                                          bool inherit = true) {
            Type attributeType = typeof(T);
            foreach (Type type in Argument.NotNull(types)) {
                IEnumerable<T> attributes = 
                    type.GetCustomAttributes(attributeType, inherit)
                        .OfType<T>();
                foreach (T attribute in attributes) {
                    yield return new KeyValuePair<Type, T>(type, attribute);
                }
            }
        }

    }

}