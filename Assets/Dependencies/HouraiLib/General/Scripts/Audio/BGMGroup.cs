using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;

#endif

namespace HouraiTeahouse {
    [CreateAssetMenu(fileName = "New BGM Group", menuName = "Hourai Teahouse/BGM Group")]
    public class BGMGroup : ScriptableObject {
        [SerializeField, Tooltip("The name of the BGM group")] private string _name;

        private WeightedRNG<BGMData> _selection;

        [SerializeField] private BGMData[] backgroundMusicData;

        public string Name {
            get { return _name; }
        }

        protected virtual void OnEnable() {
            _selection = new WeightedRNG<BGMData>();
            if (backgroundMusicData == null)
                return;
            foreach (var bgmData in backgroundMusicData) {
                bgmData.Initialize(Name);
                _selection[bgmData] = bgmData.Weight;
            }
        }

        public BGMData GetRandom() {
            return _selection.Select();
        }

#if UNITY_EDITOR

        public void SetBGMClips(IEnumerable<string> resourcePaths) {
            backgroundMusicData = resourcePaths.Select(path => new BGMData(path, 1f)).ToArray();
        }

#endif
    }

    [Serializable]
    public class BGMData {
        private const string delimiter = "/";
        private const string suffix = "weight";

        [SerializeField] [Tooltip("The artist who created this piece of music")] private string _artist;

        [SerializeField, Range(0f, 1f)] private readonly float _baseWeight = 1f;

        [SerializeField, Resource(typeof (AudioClip))] private readonly string _bgm;

        [SerializeField] [Tooltip("The sample number of the end point the loop.")] private int _loopEnd;

        [SerializeField] [Tooltip("The sample number of the start point the loop.")] private int _loopStart;

        [SerializeField] [Tooltip("The name of the BGM.")] private string _name;
        private string playerPrefsKey;

        public BGMData(string path, float weight) {
            _bgm = path;
            _baseWeight = weight;
        }

        public Resource<AudioClip> BGM { get; private set; }

        public float Weight { get; private set; }

        public int LoopStart {
            get { return _loopStart; }
        }

        public int LoopEnd {
            get { return _loopEnd; }
        }

        public void Initialize(string stageName) {
            BGM = new Resource<AudioClip>(_bgm);
            playerPrefsKey = stageName + delimiter + _bgm + "_" + suffix;

            if (Prefs.HasKey(playerPrefsKey))
                Weight = Prefs.GetFloat(playerPrefsKey);
            else {
                Prefs.SetFloat(playerPrefsKey, _baseWeight);
                Weight = _baseWeight;
            }
        }

        public override string ToString() {
            return _bgm + " - (" + _baseWeight + ")";
        }
    }
}