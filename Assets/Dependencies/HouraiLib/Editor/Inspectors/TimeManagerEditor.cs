using UnityEditor;

namespace HouraiTeahouse.Editor {

    /// <summary> 
    /// Custom Inspector for TimeManager. 
    /// </summary>
    [CustomEditor(typeof(TimeManager), true)]
    public class TimeManagerEditor : BaseEditor<TimeManager> {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            TimeManager.Paused = EditorGUILayout.Toggle("Paused", TimeManager.Paused);
            TimeManager.TimeScale = EditorGUILayout.Slider("Time Scale", TimeManager.TimeScale, 0f, 5f);
        }

    }

}