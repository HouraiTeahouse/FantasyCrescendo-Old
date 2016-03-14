using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     Handler MonoBehaviour that listens for button presses and pauses the game as needed.
    /// </summary>
    public class PauseGame : MonoBehaviour {
        /// <summary>
        ///     The button that pauses the game.
        /// </summary>
        [SerializeField] private readonly InputTarget _pauseButton = InputTarget.Start;

        /// <summary>
        ///     The player that paused the game.
        /// </summary>
        private Player _pausedPlayer;

        /// <summary>
        ///     Unity callback. Called once every frame.
        /// </summary>
        private void Update() {
            if (TimeManager.Paused) {
                if (_pausedPlayer != null && !_pausedPlayer.Controller.GetControl(_pauseButton).WasPressed)
                    return;
                _pausedPlayer = null;
                TimeManager.Paused = false;
            }
            else {
                foreach (var player in Player.ActivePlayers) {
                    if (player.Controller == null || !player.Controller.GetControl(_pauseButton).WasPressed)
                        continue;
                    _pausedPlayer = player;
                    TimeManager.Paused = true;
                    break;
                }
            }
        }
    }
}