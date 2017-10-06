using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HouraiTeahouse.SmashBrew;

namespace HouraiTeahouse.FantasyCrescendo {

    /// <summary>
    /// Plays Pause and Unpause sound effects at the approriate time.
    /// </summary>
    public class PauseSFX : MonoBehaviour {

        [SerializeField]
        AudioClip _pauseSFX;

        [SerializeField]
        AudioClip _unpauseSFX;

        [SerializeField]
        AudioSource _source;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            var context = Mediator.Global.CreateUnityContext(this);
            context.Subscribe<GamePaused>(PlayPauseEffect);
            _source = this.CachedGetComponent(_source, () => GetComponentInChildren<AudioSource>());
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() {
            _source = GetComponentInChildren<AudioSource>();
        }

        void PlayPauseEffect(GamePaused args) {
            if (_source == null)
                return;
            _source.clip = SmashTimeManager.Paused ? _pauseSFX : _unpauseSFX;
            _source.Play();
        }

    }

}
