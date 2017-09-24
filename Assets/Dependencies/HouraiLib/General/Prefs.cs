using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HouraiTeahouse {

    /// <summary> A static PlayerPrefs wrapper that provides additional type support. </summary>
    public static class Prefs {

        internal static readonly ILog log = Log.GetLogger("PlayerPrefs");

        /// <summary> Saves all the changes to disk . </summary>
        public static void Save() {
            PlayerPrefs.Save();
        }

        /// <summary> Check whether a key exists or not. </summary>
        /// <param name="key"> the key to check for </param>
        /// <returns> true, if the key exists, false otherwise </returns>
        public static bool Exists(string key) {
            return PlayerPrefs.HasKey(key);
        }

        /// <summary> Deletes the key given </summary>
        /// <param name="key"> the key to delete </param>
        public static void Delete(string key) {
            PlayerPrefs.DeleteKey(key);
        }

        static T Load<T>(string key, T value) {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode)
#endif
                log.Info("Loaded \"{0}\" : {1} ({2})", key, value, typeof(T).Name);
            return value;
        }

        #region Object Values

        /// <summary> Gets a JSON serialized object from PlayerPrefs. </summary>
        /// <remarks> The underlying representation was as a string, and can be read via <see cref="GetString" />
        /// </remarks>
        /// <seealso cref="SetObject" />
        /// <seealso cref="ReadObject" />
        /// <typeparam name="T"> the type of the object to deserialize to </typeparam>
        /// <param name="key"> the key the object is saved to </param>
        /// <returns> the deserialized object </returns>
        public static T GetObject<T>(string key) {
            return Load(key, JsonUtility.FromJson<T>(PlayerPrefs.GetString(key)));
        }

        /// <summary> Serializes an object to JSON format and saves it to PlayerPrefs. </summary>
        /// <remarks> The underlying representation was as a string, and can be read via <see cref="GetString" />
        /// </remarks>
        /// <seealso cref="GetObject{T}" />
        /// <seealso cref="ReadObject" />
        /// <param name="key"> the key to save the object to </param>
        /// <param name="obj"> the object to save to PlayerPrefs </param>
        public static void SetObject(string key, object obj) {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(obj));
        }

        /// <summary> Deserializes an object and applies it's values on an existing instance. </summary>
        /// <seealso cref="GetObject{T}" />
        /// <seealso cref="SetObject" />
        /// <param name="key"> the key the object is saved to </param>
        /// <param name="obj"> the object to apply the values to </param>
        public static void ReadObject(string key, object obj) {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), obj);
            Load(key, obj);
        }

        #endregion

        #region Bool Values

        /// <summary> Gets a boolean value from PlayerPrefs. </summary>
        /// <remarks> The underlying representation is stored as an integer. Is false if it is 0, and true otherwise. </remarks>
        /// <seealso cref="SetBool" />
        /// <param name="key"> the key the boolean value is saved to </param>
        /// <returns> the saved boolean value </returns>
        public static bool GetBool(string key) {
            return Load(key, PlayerPrefs.GetInt(key) != 0);
        }

        /// <summary> Sets a boolean value to PlayerPrefs. </summary>
        /// <remarks> The underlying representation is stored as an integer. Is false if it is 0, and true otherwise. </remarks>
        /// <seealso cref="GetBool" />
        /// <param name="key"> the key the boolean value is saved to </param>
        public static void SetBool(string key, bool value) {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        #endregion

        #region Int Values

        /// <summary> Gets an integer value from PlayerPrefs. </summary>
        /// <seealso cref="SetInt" />
        /// <param name="key"> the key the integer value is saved to </param>
        /// <returns> the saved integer value </returns>
        public static int GetInt(string key) {
            return Load(key, PlayerPrefs.GetInt(key));
        }

        /// <summary> Sets an integer value to PlayerPrefs. </summary>
        /// <seealso cref="GetInt" />
        /// <param name="key"> the key the integer value is saved to </param>
        /// <param name="value"> the value to save </param>
        public static void SetInt(string key, int value) {
            PlayerPrefs.SetInt(key, value);
        }

        #endregion

        #region Float Values

        /// <summary> Gets a floating point value from PlayerPrefs. </summary>
        /// <seealso cref="SetFloat" />
        /// <param name="key"> the key the floating point value is saved to </param>
        /// <returns> the saved floating point value </returns>
        public static float GetFloat(string key) {
            return Load(key, PlayerPrefs.GetFloat(key));
        }

        /// <summary> Sets a floating point value to PlayerPrefs </summary>
        /// <seealso cref="GetFloat" />
        /// <param name="key"> the key the floating point value is saved to </param>
        /// <param name="value"> the value to save </param>
        public static void SetFloat(string key, float value) {
            PlayerPrefs.SetFloat(key, value);
        }

        #endregion

        #region String Values

        /// <summary> Gets a string value from PlayerPrefs. </summary>
        /// <seealso cref="SetString" />
        /// <param name="key"> the key the string value is saved to </param>
        /// <returns> the saved string value </returns>
        public static string GetString(string key) {
            return Load(key, PlayerPrefs.GetString(key));
        }

        /// <summary> Sets a string value to PlayerPrefs. </summary>
        /// <seealso cref="GetString" />
        /// <param name="key"> the key the string value is saved to </param>
        /// <param name="value"> the value to save </param>
        public static void SetString(string key, string value) {
            PlayerPrefs.SetString(key, value);
        }

        #endregion
    }

}
