using System;
using System.IO;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> 
    /// Takes screenshots upon pressing a specified keyboard key
    /// </summary>
    public class Screenshot : MonoBehaviour {

        [SerializeField]
        string _dateTimeFormat = "MM-dd-yyyy-HHmmss";

        [SerializeField]
        string _format = "screenshot-{0}";

        [SerializeField]
        KeyCode _key = KeyCode.F12;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (!Input.GetKeyDown(_key))
                return;
            string filename = string.Format(_format, DateTime.UtcNow.ToString(_dateTimeFormat)) + ".png";
            string path = Path.Combine(Application.dataPath, filename);

            Debug.LogFormat("Screenshot taken. Saved to {0}", path);

            if (File.Exists(path))
                File.Delete(path);

            ScreenCapture.CaptureScreenshot(Application.platform == RuntimePlatform.IPhonePlayer ? filename : path);
        }

    }

}
