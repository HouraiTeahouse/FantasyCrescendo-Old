using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.SmashBrew.Matches {

    public class MatchConfigBuilder : MonoBehaviour {

        public static MatchConfigBuilder Instance { get; private set; }
        static MatchConfig _config;

        [SerializeField]
        int _testPlayerCount = 2;

        [SerializeField]
        int _testStockCount = 5;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Instance = this;
        }

        public void Initialize() {
            if (_config == null)
                _config = BuildTestConfig();
        }

        public MatchConfig Build() {
            if (_config == null) {
#if UNITY_EDITOR
                Debug.LogWarning("No prior match config found. Building default.");
                _config = BuildTestConfig();
#else
                throw new InvalidOperationException("Cannot start match without a match config.");
#endif
            }
            return _config;
        }

        MatchConfig BuildTestConfig() {
            var validCharacters = DataManager.Characters.Where(c => c.IsSelectable).ToArray();
            var players = new MatchPlayerConfig[_testPlayerCount];
            for (var i = 0; i < players.Length; i++) {
                Debug.LogFormat("Building test player {0}...", i + 1);
                var character = validCharacters.Random();
                players[i] = new MatchPlayerConfig{ 
                    Connection = null,
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
