using HouraiTeahouse.Localization;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    ///     A Component that displays a Character (or a Player's Character) name on a UI Text object
    /// </summary>
    public sealed class NameText : AbstractLocalizedText, IDataComponent<Player>, IDataComponent<CharacterData> {
        [SerializeField, Tooltip("Capitalize the character's name?")] private bool _capitalize;
        [SerializeField, Tooltip("The character who's name is to be displayed")] private CharacterData _character;

        private Player _player;
        private Character character;

        [SerializeField, Tooltip("Use the character's short or long name?")] private bool shortName;

        /// <summary>
        ///     <see cref="IDataComponent{CharacterData}.SetData" />
        /// </summary>
        public void SetData(CharacterData data) {
            if (data == null)
                Text.text = string.Empty;
            else
                LocalizationKey = shortName ? data.ShortName : data.FullName;
        }

        /// <summary>
        ///     <see cref="IDataComponent{Player}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            if (_player != null)
                _player.OnChanged -= OnPlayerChange;
            _player = data;
            if (_player != null)
                _player.OnChanged += OnPlayerChange;
            OnPlayerChange();
        }

        /// <summary>
        ///     <see cref="AbstractLocalizedText.Process" />
        /// </summary>
        protected override string Process(string val) {
            return !_capitalize ? val : val.ToUpperInvariant();
        }

        /// <summary>
        ///     Event callback. Called whenever the Player changes.
        /// </summary>
        private void OnPlayerChange() {
            SetData(_player == null ? null : _player.SelectedCharacter);
        }
    }
}