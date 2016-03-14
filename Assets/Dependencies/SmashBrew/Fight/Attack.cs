using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public sealed class Attack : BaseAnimationBehaviour<Character> {
        public enum Direction {
            Neutral,
            Up,
            Front,
            Back,
            Down
        }

        public enum Type {
            Normal,
            Smash,
            Aerial,
            Special
        }

        [SerializeField] private HitboxData[] _data;

        [SerializeField] private Direction _direction;

        [SerializeField] private Type _type;

        [SerializeField] private int index;

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            if (!Target)
                return;
            for (var i = 0; i < _data.Length; i++)
                _data[i].Initialize(Target, this, i);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (Target)
                Target.Attack(_type, _direction, index);

            Transition();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            var t = stateInfo.normalizedTime;
            foreach (var hitboxData in _data)
                hitboxData.Update(t, animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            Transition();
        }

        private void Transition() {
            for (var i = 0; i < _data.Length; i++)
                _data[i].OnTransition();
        }

        [Serializable]
        public class HitboxData {
            private GameObject _gameObject;
            private int _index;
            public int Bone = -1;
            public Vector3 Offset;
            public float Radius = 1f;
            public float[] TogglePoints;

            public void Initialize(Character character, Attack parent, int hitboxIndex) {
                _gameObject =
                    new GameObject("hb_" + hitboxIndex + "_" + parent._direction + "_" + parent._type + "_" +
                                   parent.index);
                var hitboxTransform = _gameObject.transform;
                hitboxTransform.parent = character.GetBone(Bone);
                hitboxTransform.localPosition = Offset;
                hitboxTransform.localRotation = Quaternion.identity;
                hitboxTransform.localScale = Vector3.one;
                _gameObject.AddComponent<SphereCollider>().radius = Radius;
                _gameObject.AddComponent<Hitbox>();
                _gameObject.SetActive(false);
                Array.Sort(TogglePoints);
            }

            public void OnTransition() {
                _gameObject.SetActive(false);
                _index = -1;
            }

            public void Update(float normalizedTime, Animator animator) {
                if (_index >= TogglePoints.Length - 1)
                    return;
                if (!(normalizedTime > TogglePoints[_index + 1]))
                    return;
                _index++;
                _gameObject.SetActive(!_gameObject.activeSelf);
            }
        }
    }
}