using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace HouraiTeahouse.SmashBrew {

    [CustomEditor(typeof(Hitbox))]
    [CanEditMultipleObjects]
    public class HitboxEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var hitboxes = targets.OfType<Hitbox>();
            var hitboxType = hitboxes.Select(h => h.CurrentType).Distinct().FirstOrDefault();
            hitboxType = (Hitbox.Type)EditorGUILayout.EnumPopup("Type", hitboxType);
            foreach (var hitbox in hitboxes) {
                serializedObject.FindProperty("_isActive").boolValue = hitboxType != Hitbox.Type.Inactive;
                serializedObject.FindProperty("_isHitbox").boolValue  = hitboxType == Hitbox.Type.Offensive;
                serializedObject.FindProperty("_isIntangible").boolValue  = hitboxType == Hitbox.Type.Intangible;
                serializedObject.FindProperty("_isInvincible").boolValue  = hitboxType == Hitbox.Type.Invincible;
                serializedObject.FindProperty("_absorbing").boolValue  = hitboxType == Hitbox.Type.Absorb;
                serializedObject.FindProperty("_reflector").boolValue  = hitboxType == Hitbox.Type.Reflective;
            }
            var isHitbox = serializedObject.FindProperty("_isHitbox");
            if (!isHitbox.boolValue)
                return;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_priority"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_damage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_baseKnockback"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_knockbackScaling"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_angle"));
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_reflectable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_absorbable"));
            }
        }
    }

}