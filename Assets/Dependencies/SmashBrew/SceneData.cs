using HouraiTeahouse.AssetBundles;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace HouraiTeahouse.SmashBrew {

    public interface IGameData {

        uint Id { get; }

        // Is the data selectable?
        bool IsSelectable { get; }

        // Is the data visible 
        bool IsVisible { get; }

        // Unloads all data associated with the data
        void Unload();

    }

    public enum SceneType {
        Other = 0,
        Stage = 1,
        Menu = 2
    }

    [CreateAssetMenu(fileName = "New Stage", menuName = "SmashBrew/Scene Data")]
    public class SceneData : BGMGroup, IGameData {

        [SerializeField]
        [ReadOnly]
        [Tooltip("The unique ID used for this scene")]
        uint _id;

        [Header("Load Data")]
        [SerializeField]
        [Tooltip("What kind of scene is it?")]
        SceneType _type;

        [SerializeField]
        [Tooltip("The priority in loading in dynamic enviroments.")]
        int _loadPriority;

        [SerializeField]
        [Resource(typeof(Sprite))]
        [Tooltip("The icon used shown on menus to represent the scene.")]
        string _icon;

        [SerializeField]
        [Tooltip("Is this scene selectable on select screens?")]
        bool _isSelectable = true;

        [SerializeField]
        [Tooltip("Is this scene visible on select screens?")]
        bool _isVisible = true;

        [SerializeField]
        bool _isDebug;

        [SerializeField]
        [Resource(typeof(Sprite))]
        [Tooltip("The image shown on menus to represent the scene.")]
        string _previewImage;

        [SerializeField]
        [Scene]
        [Tooltip("The internal name of the scene. Must be in build settings.")]
        string _scene;

        public uint Id {
            get { return   _id;}
        }

        /// <summary> 
        /// The image shown on menus to represent the scene. 
        /// </summary>
        public Resource<Sprite> PreviewImage {
            get { return Resource.Get<Sprite>(_previewImage);}
        }

        /// <summary> 
        /// The icon used on menus to represent the scene. 
        /// </summary>
        public Resource<Sprite> Icon {
            get { return Resource.Get<Sprite>(_icon);}
        }

        /// <summary> 
        /// Is the scene described by this SceneData a stage? 
        /// </summary>
        public SceneType Type {
            get { return _type;}
        }

        public int LoadPriority {
            get { return _loadPriority;}
        }
        public bool IsSelectable {
            get { return _isSelectable && IsVisible;}
        }
        public bool IsVisible {
            get { return _isVisible && (Debug.isDebugBuild || !_isDebug);}
        }

        public void Unload() {
            if(PreviewImage != null)
                PreviewImage.Unload();
            if (Icon != null)
                Icon.Unload();
        }

        /// <summary> 
        /// Loads the scene described by the SceneData 
        /// </summary>
        public ITask Load() {
            var task = SceneLoader.LoadScene(_scene);
            task.Then(() => Mediator.Global.Publish(new LoadSceneEvent { Scene = this }));
            return task;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected override void OnDisable() {
            base.OnDisable();
            Unload();
        }

        void Reset() {
            RegenerateID();
        }

        [ContextMenu("Regenerate ID")]
        void RegenerateID() { 
            _id = (uint)new Random().Next();
        }

    }

}
