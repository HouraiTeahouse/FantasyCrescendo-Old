using HouraiTeahouse.Localization;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A Component that displays a Character (or a Player's Character) name on a UI Text object </summary>
    public sealed class NameText : AbstractLocalizedText, IDataComponent<Player>, IDataComponent<CharacterData>, IDataComponent<SceneData> {

        [SerializeField]
        [Tooltip("Capitalize the character's name?")]
        bool _capitalize;

        [SerializeField]
        [Tooltip("The character who's name is to be displayed")]
        CharacterData _character;

        [SerializeField]
        [Tooltip("The scene which name is to be displayed")]
        SceneData _scene;

        Player _player;

        [SerializeField]
        [Tooltip("Use the character's short or long name?")]
        bool shortName;

        /// <summary>
        ///     <see cref="IDataComponent{CharacterData}.SetData" />
        /// </summary>
        public void SetData(SceneData data) {
            if (data != null)
                NativeText = data.Name;
        }

        /// <summary>
        ///     <see cref="IDataComponent{CharacterData}.SetData" />
        /// </summary>
        public void SetData(CharacterData data) {
            if (data != null)
                NativeText = shortName ? data.ShortName : data.FullName;
        }

        /// <summary>
        ///     <see cref="IDataComponent{Player}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            _player = data;
            PlayerChange();
        }

        protected override void Awake() {
            base.Awake();
            SetData(_character);
            SetData(_scene);
            Mediator.Global.CreateUnityContext(this)
                    .Subscribe<PlayerChanged>(args => {
                        if (args.Player == _player)
                            PlayerChange();
                    });
        }

        /// <summary>
        ///     <see cref="AbstractLocalizedText.Process" />
        /// </summary>
        protected override string Process(string val) {
            return !_capitalize ? val : val.ToUpperInvariant();
        }

        /// <summary> 
        /// Events callback. Called whenever the Player changes. 
        /// </summary>
        void PlayerChange() { 
           SetData(_player == null ? null : _player.Selection.Character); 
        }

    }

}
