using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Audio;

namespace HouraiTeahouse {
    /// <summary>
    ///     A controllable audio channel
    /// </summary>
    [Serializable]
    public sealed class AudioChannel {
        [SerializeField, Tooltip("The associated exposed parameters on the Audio Mixer that are to be changed.")] private string[] _associatedParams;

        [SerializeField, Tooltip("The default volume for the channel")] private readonly float _baseVolume = 1f;
        private float _currentVolume;

        private AudioMixer _mixer;

        [SerializeField, Tooltip("The viewable name for the channel. May be used for in-game UI elements.")] private
            string _name;

        [SerializeField, Tooltip("The PlayerPrefs key to save the volume data onto")] private string _playerPrefsKey;

        /// <summary>
        ///     Gets or sets the AudioChannel's current volume. Editing this volume will make it louder or softer.
        ///     Range of [0..1].
        /// </summary>
        public float CurrentVolume {
            get { return _currentVolume; }
            set {
                _currentVolume = value;
                foreach (var param in _associatedParams)
                    _mixer.SetFloat(param, value);
            }
        }

        /// <summary>
        ///     Gets the viewable name for the channel.
        /// </summary>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        ///     Initializes the AudioChannel. Retrieves volume data from PlayerPrefs or sets it to a default value if it doesn't
        ///     exist.
        /// </summary>
        /// <param name="mixer">the main Audio mixer for the game</param>
        internal void Initialize(AudioMixer mixer) {
            _mixer = mixer;
            if (!Prefs.HasKey(_playerPrefsKey)) {
                Prefs.SetFloat(_playerPrefsKey, _baseVolume);
                CurrentVolume = _baseVolume;
            }
            else {
                CurrentVolume = Prefs.GetFloat(_playerPrefsKey);
            }
        }

        /// <summary>
        ///     Saves the current volume of the channel to PlayerPrefs
        /// </summary>
        internal void Save() {
            Prefs.SetFloat(_playerPrefsKey, _currentVolume);
        }
    }

    /// <summary>
    ///     A singleton wrapper for the master AudioMixer to provide easier programmatic control over defined audio channels
    /// </summary>
    public sealed class AudioManager : MonoBehaviour {
        [SerializeField, Tooltip("The controllable defined channels1")] private AudioChannel[] _audioChannels;
        private Dictionary<string, AudioChannel> _channelByName;

        [SerializeField, Tooltip("The editable audio mixer")] private AudioMixer _mixer;

        /// <summary>
        ///     Singleton instance of AudioManager. If null, there does not exist one in the scene.
        /// </summary>
        public static AudioManager Instance { get; private set; }

        /// <summary>
        ///     A collection of the Channels defined in the editor.
        /// </summary>
        public ReadOnlyCollection<AudioChannel> Channels { get; private set; }

        /// <summary>
        ///     Indexer to get the channel corresponding to it's name.
        /// </summary>
        /// <param name="name">the name of the channel</param>
        /// <returns>the corresponding audio channel</returns>
        public AudioChannel this[string name] {
            get { return _channelByName[name]; }
        }

        /// <summary>
        ///     Unity Callback. Called on object instantiation.
        /// </summary>
        private void Awake() {
            Instance = this;
            Channels = new ReadOnlyCollection<AudioChannel>(_audioChannels);
            _channelByName = new Dictionary<string, AudioChannel>();
            if (_audioChannels == null || _mixer == null)
                return;
            foreach (var channel in _audioChannels) {
                if (channel == null) continue;
                channel.Initialize(_mixer);
                _channelByName[channel.Name] = channel;
            }
        }

        /// <summary>
        ///     Unity Callback. Called on object destruction.
        /// </summary>
        private void OnDestroy() {
            Save();
        }

        /// <summary>
        ///     Unity Callback. Called when the entire application is exited.
        /// </summary>
        private void OnApplicationQuit() {
            Save();
        }

        /// <summary>
        ///     Saves all of the channels to PlayerPrefs to allow for persistence.
        /// </summary>
        private void Save() {
            foreach (var channel in _audioChannels)
                if (channel != null)
                    channel.Save();
        }
    }
}