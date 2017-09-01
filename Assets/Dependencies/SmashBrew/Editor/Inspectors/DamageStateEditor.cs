using HouraiTeahouse.SmashBrew.Characters;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew {

    [CustomEditor(typeof(DamageState))]
    public class DamageStateEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var damage = target as DamageState;
            if (!EditorApplication.isPlayingOrWillChangePlaymode || damage.Character == null)
                return;
            damage.Character.State.Damage = EditorGUILayout.FloatField("Current Damage", 
                damage.Character.State.Damage);
        }

    }

}

