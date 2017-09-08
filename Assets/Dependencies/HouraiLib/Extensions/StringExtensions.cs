using System;

namespace HouraiTeahouse {

    public static class StringExtensions {

        /// <summary> 
        /// Generic function that will parse a string into an enum value. 
        /// </summary>
        /// <typeparam name="T"> the type of enum to parse into </typeparam>
        /// <param name="str"> the string to parse </param>
        /// <param name="ignoreCase"> whether or not to ignore case when parsing </param>
        /// <returns> the enum value </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> is null </exception>
        public static T ToEnum<T>(this string str, bool ignoreCase = false)
            where T : struct, IComparable, IFormattable, IConvertible {
            Argument.Check(str.IsNullOrEmpty());
            return (T) Enum.Parse(typeof(T), Argument.NotNull(str), ignoreCase);
        }

        /// <summary> 
        /// Shorthand for string.Format. 
        /// </summary>
        /// <param name="str"> string to format </param>
        /// <param name="objs"> the objects to format it with </param>
        /// <returns> the formatted string </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> or <paramref name="objs" /> is null </exception>
        public static string With(this string str, params object[] objs) {
            return string.Format(Argument.NotNull(str), objs);
        }

        /// <summary>
        /// Replaces an null string with an empty one.
        /// </summary>
        /// <param name="str"> the provided string </param>
        /// <returns> a complete string or an empty one </returns>
        public static string EmptyIfNull(this string str) { 
            return str ?? string.Empty; 
        }

    }

}
