using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.SmashBrew.Matches {

    public class MatchStarter : MonoBehaviour {

        public static MatchConfig MatchConfig;

        [SerializeField]
        int _testPlayerCount = 1;

        [SerializeField]
        int _testStockCount = 5;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            MatchConfig config = MatchConfig;
            SmashNetworkManager.Instance.StartHost();
            if (config == null) {
#if UNITY_EDITOR
                Log.Warning("No prior match config found. Building default.");
                config = BuildTestConfig();
#else
                throw new InvalidOperationException("Cannot start match without a match config.");
#endif
            }
            if (Match.Current == null)
                throw new InvalidOperationException("A match instance cannot be found!");
            Match.Current.Initialize(config);
        }

        MatchConfig BuildTestConfig() {
            var validCharacters = DataManager.Characters.Where(c => c.IsSelectable).ToArray();
            var players = new MatchPlayerConfig[_testPlayerCount];
            for (var i = 0; i < players.Length; i++) {
                Log.Debug("Building test player {0}...", i + 1);
                var character = validCharacters.Random();
                players[i] = new MatchPlayerConfig{ 
                    Connection = NetworkServer.connections[0],
                    PlayerControllerId = (short)i,
                    Type = PlayerType.HumanPlayer,
                    Selection = new PlayerSelection {
                        Character = character,
                        Pallete = Mathf.FloorToInt(Random.value * character.PalleteCount)
                    }
                };
            }
            return new MatchConfig {
                PlayerSelections = players,
                Stocks = _testStockCount
            };
        }

    }

}
