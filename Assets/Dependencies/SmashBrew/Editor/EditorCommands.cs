using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.SmashBrew {

    class EditorCommands {

        [MenuItem("Smash Brew/Add Offensive Hitbox %h")]
        static void AddOffensiveHitbox() {
            AddHitbox(Hitbox.Type.Offensive);
        }

        [MenuItem("Smash Brew/Add Hurtbox %#h")]
        static void AddHurtbox() {
            AddHitbox(Hitbox.Type.Damageable);
        }

        static Hitbox CreateHitbox(Hitbox.Type type, Transform parent) {
            var hbGo = new GameObject();
            Undo.RegisterCreatedObjectUndo(hbGo, "Create Hitbox GameObject");
            var collider = Undo.AddComponent<SphereCollider>(hbGo);
            var hb = Undo.AddComponent<Hitbox>(hbGo);
            hb.CurrentType = type;
            if (parent != null) {
                Undo.SetTransformParent(hb.transform, parent, "Parent Hitbox");
            }
            hb.transform.Reset();
            Undo.RecordObject(collider, "Edit Collider Size");
            collider.radius = 1f / ((Vector3) (hb.transform.localToWorldMatrix * Vector3.one)).Max();
            return hb;
        }

        static GameObject GetRootOrCharacterGameObject(Component component) {
            var character = component.GetComponentInParent<Character>();
            var root = component.transform.root;
            if (character != null)
                return character.gameObject;
            else if (root == null)
                return component.transform.gameObject;
            return root.gameObject;
        }

        static void AddHitboxToSet(Dictionary<GameObject, List<Hitbox>> rootMap, GameObject rootGo, Hitbox hitbox) {
            if (!rootMap.ContainsKey(rootGo))
                rootMap[rootGo] = new List<Hitbox>();
            rootMap[rootGo].Add(hitbox);
        }

        static void AddHitbox(Hitbox.Type type) {
            var hitboxes = new List<Hitbox>();
            Undo.IncrementCurrentGroup();
            if (Selection.gameObjects.Length <= 0)  {
                var hitbox = CreateHitbox(type, null);
                hitboxes.Add(hitbox);
                hitbox.name = string.Format("hb_{0}", type);
            } else {
                var rootMap = new Dictionary<GameObject, List<Hitbox>>();
                foreach (GameObject gameObject in Selection.gameObjects) {
                    var hitbox = CreateHitbox(type, gameObject.transform);
                    hitboxes.Add(hitbox);
                    AddHitboxToSet(rootMap, GetRootOrCharacterGameObject(hitbox), hitbox);
                }
                foreach (KeyValuePair<GameObject, List<Hitbox>> set in rootMap) {
                    Hitbox[] allHitboxes = set.Key.GetComponentsInChildren<Hitbox>();
                    int i = allHitboxes.Length - set.Value.Count;
                    Undo.RecordObjects(set.Value.ToArray(), "Name Changes");
                    foreach (Hitbox hitbox in set.Value) {
                        hitbox.name = string.Format("{0}_hb_{1}_{2}", set.Key.name, type, i).ToLower();
                        i++;
                    }
                }
            }
            Selection.objects = hitboxes.GetGameObject().ToArray();
            Undo.SetCurrentGroupName(string.Format("Generate {0} Hitbox{1}",
                type,
                hitboxes.Count > 0 ? "es" : string.Empty));
        }

    }

}
