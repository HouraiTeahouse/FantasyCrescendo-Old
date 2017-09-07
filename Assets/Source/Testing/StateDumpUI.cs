using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.Characters;

namespace HouraiTeahouse.FantasyCrescendo {

    /// <summary>
    /// In-Game Debug UI used to show the state.
    /// Intended only for use on the Debug Stage.
    ///  </summary>
    public class StateDumpUI : MonoBehaviour {

        [SerializeField]
        Text _text;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            _text = GetComponent<Text>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (_text == null)
                return;
            var manager = PlayerManager.Instance;
            if (manager == null) {
                _text.text = string.Empty;
                return;
            }
            var builder = new StringBuilder();
            foreach(var player in manager.MatchPlayers) {
                if (player.PlayerObject != null) {
                    var character = player.PlayerObject.GetComponentInChildren<Character>();
                    if (character != null) {
                        builder.AppendLine("P{0}: {1}".With(player.ID + 1, character.StateController.CurrentState.Name));
                    }
                } else {
                    builder.AppendLine("P{0}: NONE".With(player.ID + 1));
                }
            }
            _text.text = builder.ToString();
        }

    }

}

