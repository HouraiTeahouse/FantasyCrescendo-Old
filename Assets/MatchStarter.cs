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
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            MatchConfig config = MatchConfig;
            var client = SmashNetworkManager.Instance.StartHost();
            NetworkServer.SpawnObjects();
            NetworkServer.dontListen = true;
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
            foreach (var localPlayer in config.PlayerSelections)
                ClientScene.AddPlayer(client.connection, localPlayer.PlayerControllerId);
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
