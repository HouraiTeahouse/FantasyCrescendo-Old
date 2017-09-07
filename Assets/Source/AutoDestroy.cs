using UnityEngine;

namespace HouraiTeahouse {

    public class AutoDestroy : MonoBehaviour {

        Animation _animation;
        AudioSource _audio;
        ParticleSystem _particleSystem;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            _animation = GetComponent<Animation>();
            _audio = GetComponent<AudioSource>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (_animation && _animation.isPlaying)
                return;
            if (_audio && _audio.isPlaying)
                return;
            if (_particleSystem && _particleSystem.isPlaying)
                return;
            Destroy(gameObject);
        }

    }

}