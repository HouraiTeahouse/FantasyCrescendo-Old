using UnityEngine;

namespace HouraiTeahouse {
    [RequireComponent(typeof (AudioSource))]
    public sealed class SoundEffect : HouraiBehaviour {
        private bool destroyOnFinish;

        public AudioSource Audio { get; private set; }

        public float Pitch { get; set; }

        protected override void Awake() {
            base.Awake();
            Audio = GetComponent<AudioSource>();
            Pitch = Audio.pitch;
        }

        private void Update() {
            Audio.pitch = EffectiveTimeScale * Pitch;
            if (destroyOnFinish && !Audio.isPlaying)
                Destroy(gameObject);
        }

        public AudioSource Play() {
            return Play(Vector3.zero);
        }

        public AudioSource Play(float volume) {
            var audioSource = Play();
            audioSource.volume = volume;
            return audioSource;
        }

        public AudioSource Play(Vector3 position) {
            var soundEffect = Instantiate(this, position, Quaternion.identity) as SoundEffect;
            soundEffect.destroyOnFinish = true;
            return soundEffect.Audio;
        }

        public AudioSource Play(float volume, Vector3 position) {
            var audioSource = Play(position);
            audioSource.volume = volume;
            return audioSource;
        }
    }
}