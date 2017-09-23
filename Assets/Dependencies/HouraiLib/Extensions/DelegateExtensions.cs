using System;
using System.Collections.Generic;

namespace HouraiTeahouse {

    public static class DelegateExtensions {

        /// <summary> 
        /// Memoizes a function. Makes it easy to call an expensive funciton multiple times. This function assumes the
        /// result is immutable and does not change over time. Note that the results passed back by the function will not be
        /// garbage collected until the retunred memoized function falls out of scope. 
        /// </summary>
        /// <typeparam name="T"> the return type of the function </typeparam>
        /// <param name="func"> the function to memoize </param>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null </exception>
        /// <returns> the memoized function </returns>
        public static Func<T> Memoize<T>(this Func<T> func) {
            Argument.NotNull(func);
            object cache = null;
            return delegate {
                if (cache == null)
                    cache = func();
                return (T) cache;
            };
        }

        /// <summary> 
        /// Memoizes a function. Makes it easy to call an expensive funciton multiple times. This function assumes the
        /// result is immutable and does not change over time. Note that the results passed back by the function will not be
        /// garbage collected until the retunred memoized function falls out of scope. 
        /// </summary>
        /// <param name="func"> the function to memoize </param>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null </exception>
        /// <returns> the memoized function </returns>
        /// <typeparam name="T"> the function's argument type </typeparam>
        /// <typeparam name="TResult"> the return type of the function </typeparam>
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func) {
            Argument.NotNull(func);
            var cache = new Dictionary<T, TResult>();
            return delegate(T val) {
                if (!cache.ContainsKey(val))
                    cache[val] = func(val);
                return cache[val];
            };
        }

        /// <summary> 
        /// Memoizes a function. Makes it easy to call an expensive funciton multiple times. This function assumes the
        /// result is immutable and does not change over time. Note that the results passed back by the function will not be
        /// garbage collected until the retunred memoized function falls out of scope. 
        /// </summary>
        /// <param name="func"> the function to memoize </param>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null </exception>
        /// <returns> the memoized function </returns>
        /// <typeparam name="T1"> the function's first argument type </typeparam>
        /// <typeparam name="T2"> the function's second argument type </typeparam>
        /// <typeparam name="TResult"> the return type of the function </typeparam>
        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> func) {
            Argument.NotNull(func);
            var cache = new Table2D<T1, T2, TResult>();
            return delegate(T1 arg1, T2 arg2) {
                if (!cache.ContainsKey(arg1, arg2))
                    cache[arg1, arg2] = func(arg1, arg2);
                return cache[arg1, arg2];
            };
        }

    }

}