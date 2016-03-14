using System.Linq;
using UnityEngine;

namespace HouraiTeahouse {
    public interface ITimeObject {
        float LocalTimeScale { get; set; }
    }

    [DisallowMultipleComponent]
    public sealed class TimeModifier : MonoBehaviour, ITimeObject {
        private Animator[] _animators;
        private float _localTimeScale = 1f;

        public float EffectiveTimeScale {
            get { return Time.timeScale * _localTimeScale; }
        }

        public float DeltaTime {
            get { return Time.deltaTime * _localTimeScale; }
        }

        public float FixedDeltaTime {
            get { return Time.fixedDeltaTime * _localTimeScale; }
        }

        //TODO: Figure out how to get this working with particle system
        //private ParticleSystem[] particles;

        public float LocalTimeScale {
            get { return _localTimeScale; }
            set {
                _localTimeScale = value;
                if (_animators.Length <= 0)
                    return;
                foreach (var animator in _animators.Where(animator => animator != null))
                    animator.speed = value;
            }
        }

        private void Awake() {
            _animators = GetComponentsInChildren<Animator>();
            if (_animators.Length <= 0)
                _animators = null;
            //particles = GetComponentsInChildren<ParticleSystem>();
            //if (particles.Length <= 0)
            //    particles = null;
        }
    }
}