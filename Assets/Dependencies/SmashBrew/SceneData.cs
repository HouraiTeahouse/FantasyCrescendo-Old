using HouraiTeahouse.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HouraiTeahouse.SmashBrew {
    [CreateAssetMenu(fileName = "New Stage", menuName = "SmashBrew/Scene Data")]
    public class SceneData : BGMGroup {
        [SerializeField] [Resource(typeof (Sprite))] [Tooltip("The icon used shown on menus to represent the scene.")] private string _icon;

        [SerializeField] [Tooltip("Is this scene a stage?")] private readonly bool _isStage = true;

        [SerializeField] [Resource(typeof (Sprite))] [Tooltip("The image shown on menus to represent the scene.")] private string _previewImage;

        /// <summary>
        ///     The internal name of the scene. Must be in build settings.
        /// </summary>
        [SerializeField, Scene] [Tooltip("The internal name of the scene. Must be in build settings.")] private string
            _sceneName;

        /// <summary>
        ///     The image shown on menus to represent the scene.
        /// </summary>
        public Resource<Sprite> PreviewImage { get; private set; }

        /// <summary>
        ///     The icon used on menus to represent the scene.
        /// </summary>
        public Resource<Sprite> Icon { get; private set; }

        /// <summary>
        ///     Is the scene described by this SceneData a stage?
        /// </summary>
        public bool IsStage {
            get { return _isStage; }
        }

        /// <summary>
        ///     Loads the scene described by the SceneData
        /// </summary>
        public void Load() {
            GlobalMediator.Instance.Publish(new LoadSceneEvent {
                LoadOperation = SceneManager.LoadSceneAsync(_sceneName),
                Scene = this
            });
        }

        /// <summary>
        ///     Unity Callback. Called when ScriptableObject is loaded.
        /// </summary>
        protected override void OnEnable() {
            base.OnEnable();
            PreviewImage = new Resource<Sprite>(_previewImage);
            Icon = new Resource<Sprite>(_icon);
        }
    }
}