using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.Characters;
using HouraiTeahouse.SmashBrew.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

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
            var match = Match.Current;
            if (match == null) {
                _text.text = string.Empty;
                return;
            }
            var builder = new StringBuilder();
            foreach(var player in match.Players) {
                if (player.PlayerObject != null) {
                    var character = player.PlayerObject.GetComponentInChildren<Character>();
                    if (character != null) {
                        builder.AppendLine(string.Format("P{0}: {1}", player.ID + 1, character.StateController.CurrentState.Name));
                    }
                } else {
                    builder.AppendLine(string.Format("P{}: NONE", player.ID + 1));
                }
            }
            _text.text = builder.ToString();
        }

    }

}

