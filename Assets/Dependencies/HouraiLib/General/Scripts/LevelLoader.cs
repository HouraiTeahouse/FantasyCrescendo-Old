using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse {
    /// <summary>
    ///     A SingleActionBehaviour that loads new Scenes
    /// </summary>
    public class LevelLoader : SingleActionBehaviour {
        [SerializeField, Tooltip("Ignore if scenes are already loaded?")] private bool _ignoreLoadedScenes;

        [SerializeField, Tooltip("The mode to load scenes in")] private readonly LoadSceneMode _mode =
            LoadSceneMode.Single;

        [SerializeField, Scene, Tooltip("The target scenes to load")] private string[] _scenes;

        /// <summary>
        ///     The paths of the scenes to load
        /// </summary>
        public string[] Scenes {
            get { return _scenes; }
            set { _scenes = value; }
        }

        /// <summary>
        ///     <see cref="SingleActionBehaviour.Action" />
        /// </summary>
        protected override void Action() {
            Load();
        }

        /// <summary>
        ///     Loads the scenes
        /// </summary>
        public void Load() {
            var paths = new HashSet<string>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
                paths.Add(SceneManager.GetSceneAt(i).path);
            foreach (var scenePath in _scenes) {
                if (!_ignoreLoadedScenes && paths.Contains(string.Format("Assets/{0}.unity", scenePath)))
                    continue;
                SceneManager.LoadScene(scenePath, _mode);
            }
        }
    }
}