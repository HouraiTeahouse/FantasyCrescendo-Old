using HouraiTeahouse.HouraiInput;
using HouraiTeahouse.SmashBrew.Matches;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> 
    /// Handler MonoBehaviour that listens for button presses and pauses the game as needed. 
    /// </summary>
    public class PauseGame : MonoBehaviour {

        [SerializeField]
        [Tooltip("The button that pauses the game.")]
        InputTarget _pauseButton = InputTarget.Start;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            // Assure that all players are local before allowing 
            var match = Match.Current;
            if (match == null)
                return;
            var allLocal = match.Players
                            .Where(p => p.Type.IsActive && p.NetworkIdentity != null)
                            .All(p => p.NetworkIdentity.isLocalPlayer);
            if (!allLocal) {
                TimeManager.Paused = false;
                return;
            }
            if (TimeManager.Paused) {
                Assert.IsNotNull(SmashTimeManager.PausedPlayer);
                Player player = SmashTimeManager.PausedPlayer;
                var unpause = player != null && player.Controller.GetControl(_pauseButton).WasPressed;
                unpause |= Input.GetKeyDown(KeyCode.Return) && player == match.Players.Where(p => p.Type.IsActive).First();
                if (!unpause)
                    return;
                SmashTimeManager.PausedPlayer = null;
                Debug.LogFormat("Game unpaused by {0}.", player);
            } else {
                foreach (Player player in match.Players.Where(p => p.Type.IsActive)) {
                    if (player.Controller == null || !player.Controller.GetControl(_pauseButton).WasPressed)
                        continue;
                    SmashTimeManager.PausedPlayer = player;
                    Debug.LogFormat("Game paused by {0}.", player);
                    break;
                }
                if (Input.GetKeyDown(KeyCode.Return)) {
                    var player = match.Players.Where(p => p.Type.IsActive).First();
                    SmashTimeManager.PausedPlayer = player;
                    Debug.LogFormat("Game paused by {0}.", player);
                }
            }
        }

    }

}
