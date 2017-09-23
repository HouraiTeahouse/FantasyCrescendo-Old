using UnityEngine;

namespace HouraiTeahouse {

    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundEffect : MonoBehaviour {

        AudioSource _audio;
        TimeModifier _timeModifier;

        bool destroyOnFinish;

        public AudioSource Audio => _audio;

        public float Pitch { get; set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            _audio = GetComponent<AudioSource>();
            _timeModifier = GetComponentInParent<TimeModifier>();
            if (_timeModifier == null)
                _timeModifier = gameObject.AddComponent<TimeModifier>();
            Pitch = _audio.pitch;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            _audio.pitch = _timeModifier.EffectiveTimeScale * Pitch;
            if (destroyOnFinish && !_audio.isPlaying)
                Destroy(gameObject);
        }

        public AudioSource Play() { 
            return Play(Vector3.zero); 
        }

        public AudioSource Play(float volume) {
            AudioSource audioSource = Play();
            audioSource.volume = volume;
            return audioSource;
        }

        public AudioSource Play(Vector3 position) {
            var soundEffect = Instantiate(this, position, Quaternion.identity) as SoundEffect;
            soundEffect.destroyOnFinish = true;
            return soundEffect.Audio;
        }

        public AudioSource Play(float volume, Vector3 position) {
            AudioSource audioSource = Play(position);
            audioSource.volume = volume;
            return audioSource;
        }

    }

}
