using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary> 
    /// Custom Editor for AudioSourceControl. 
    /// </summary>
    [CustomEditor(typeof(AudioSourceControl))]
    internal class AudioSourceControlInspector : BaseEditor<AudioSourceControl> {

        public override void OnInspectorGUI() {
            var source = Target.GetComponent<AudioSource>();
            using (new EditorGUILayout.HorizontalScope()) {
                GUI.enabled = EditorApplication.isPlayingOrWillChangePlaymode;
                if (GUILayout.Button("Play"))
                    source.Play();
                if (GUILayout.Button("Pause"))
                    source.Pause();
                if (GUILayout.Button("Stop"))
                    source.Stop();
                GUI.enabled = true;
            }
        }

    }

}