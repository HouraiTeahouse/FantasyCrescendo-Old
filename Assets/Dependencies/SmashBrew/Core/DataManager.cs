using System.Collections.Generic;
using System.Collections.ObjectModel;
using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     A manager of all of the game data loaded into the game.
    /// </summary>
    public sealed class DataManager : MonoBehaviour {
        [SerializeField, Tooltip("The characters to display in the game")] private List<CharacterData> _characters;
        [SerializeField, Tooltip("Destroy this instance on scene loads?")] private bool _dontDestroyOnLoad;

        [SerializeField, Tooltip("The scenes to show in the game")] private List<SceneData> _scenes;

        public Mediator Mediator { get; private set; }

        /// <summary>
        ///     The Singleton instance of DataManager.
        /// </summary>
        public static DataManager Instance { get; private set; }

        /// <summary>
        ///     All Characters that are included with the Game's build.
        ///     The Data Manager will automatically load all CharacterData instances from Resources.
        /// </summary>
        public ReadOnlyCollection<CharacterData> Characters { get; private set; }

        /// <summary>
        ///     All Scenes and their metadata included with the game's build.
        ///     The DataManager will automatically load all SceneData instances from Resources.
        /// </summary>
        public ReadOnlyCollection<SceneData> Scenes { get; private set; }

        /// <summary>
        ///     Unity Callback. Called on object instantion.
        /// </summary>
        private void Awake() {
            Instance = this;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);

            Mediator = GlobalMediator.Instance;

            Characters = new ReadOnlyCollection<CharacterData>(_characters);
            Scenes = new ReadOnlyCollection<SceneData>(_scenes);
        }
    }
}