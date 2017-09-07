using HouraiTeahouse.SmashBrew;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    /// <summary>
    /// Controller for the Music Room UI.
    /// </summary>
    public class MusicRoomController : MonoBehaviour {

        BGMData[] _bgmData;

        [SerializeField]
        PlayBGM _bgmPlayer;

        [SerializeField]
        [Tooltip("The UI Text to display the name of the ")]
        GameObject _textSource;

        int _currentIndex;

        public event Action<BGMData> OnBGMChange;

        /// <summary>
        /// Gets the currently selected BGMData.
        /// </summary>
        /// <returns> the currently selected BGM </returns>
        public BGMData CurrentSelectedBGM {
            get { return _bgmData[_currentIndex]; }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (DataManager.Scenes == null)
                _bgmData = new BGMData[0];
            else
                _bgmData = DataManager.Scenes.Where(scene => scene.IsSelectable)
                                             .SelectMany(scene => scene.MusicData).ToArray();
            SetViewText();
        }

        /// <summary>
        /// Changes the provided music along the reel.
        /// Note: this will not cause the newly selected music to play.
        /// </summary>
        /// <param name="distance"> the distance along the reel to move. </param>
        public void ChangeMusic(int distance) {
            _currentIndex = (_currentIndex + distance) % _bgmData.Length;
            while(_currentIndex < 0)
                _currentIndex += _bgmData.Length;
            OnBGMChange.SafeInvoke(CurrentSelectedBGM);
            SetViewText();
        }

        /// <summary>
        /// Plays the currently selected BGM.
        /// </summary>
        public void PlayCurrent() {
            _bgmPlayer.Play(CurrentSelectedBGM);
        }

        void SetViewText() {
            if (_textSource == null)
                return;
            var bgm = CurrentSelectedBGM;
            var text = "{0}\nby {1}".With(bgm.Name, bgm.Artist);
            if (!string.IsNullOrEmpty(bgm.OriginalName))
                text += "\nOriginal: " + bgm.OriginalName;
            _textSource.SetUIText(text);
        }

    }

}
