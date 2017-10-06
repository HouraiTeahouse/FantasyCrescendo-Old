using System;
using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> 
    /// An interface for defining data driven components. 
    /// </summary>
    public interface IDataComponent<in T> {

        /// <summary> 
        /// Sets the data for the IDataComponent 
        /// </summary>
        /// <param name="data"> the new data to set </param>
        void SetData(T data);

    }

    /// <summary> 
    /// Extension methods for IDataComponent and collections of IDataComponents 
    /// </summary>
    public static class DataComponentExtensions {

        /// <summary> 
        /// Sets the data on all elements in a collection of IDataComponent 
        /// </summary>
        /// <typeparam name="T"> the type of data </typeparam>
        /// <param name="enumeration"> the colleciton of IDataComponents </param>
        /// <exception cref="ArgumentNullException"> <paramref name="enumeration" /> is null </exception>
        /// <param name="data"> the data to be set </param>
        public static void SetData<T>(this IEnumerable<IDataComponent<T>> enumeration, T data) {
            foreach (IDataComponent<T> dataComponent in Argument.NotNull(enumeration).IgnoreNulls())
                dataComponent.SetData(data);
        }

    }

}