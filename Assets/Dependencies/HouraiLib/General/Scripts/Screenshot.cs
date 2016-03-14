using System;
using System.IO;
using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    ///     Takes screenshots upon pressing F12
    /// </summary>
    //TODO: Generalize
    public class Screenshot : MonoBehaviour {
        /// <summary>
        ///     Unity callback. Called once per frame.
        /// </summary>
        private void Update() {
            if (Input.GetKeyDown(KeyCode.F12)) {
                var filename = "screenshot-" + DateTime.UtcNow.ToString("MM-dd-yyyy-HHmmss") + ".png";
                var path = Path.Combine(Application.persistentDataPath, filename);

                if (File.Exists(path))
                    File.Delete(path);

                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    Application.CaptureScreenshot(filename);
                else
                    Application.CaptureScreenshot(path);
            }
        }
    }
}