using UnityEditor;
using UnityEngine;

namespace HouraiTeahouse.Editor {

    /// <summary> Custom Editor for TimeModifier. </summary>
    [CustomEditor(typeof(TimeModifier))]
    internal class TimeModifierEditor : BaseEditor<TimeModifier> {

        /// <summary>
        ///     <see cref="UnityEditor.Editor.OnInspectorGUI" />
        /// </summary>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            using (HGUI.Enabled(Target != null && !EditorApplication.isPlayingOrWillChangePlaymode)) {
                Target.LocalTimeScale = EditorGUILayout.FloatField("Time Scale", Target.LocalTimeScale);
            }
        }

    }

}
