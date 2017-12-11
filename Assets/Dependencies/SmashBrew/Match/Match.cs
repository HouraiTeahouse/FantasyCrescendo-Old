using HouraiTeahouse.SmashBrew.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.SmashBrew.Matches {

    public enum MatchStatus {
        Initialization = 0,     // Initializing and setting up match.
        WaitingOnServer,        // Wait on server to initialize
        Spawning,               // Match is spawning characters, players do not currently have control.
        Running,                // Players currently have control of their characters.
        Completed,              // Match completed. Waiting on resolution.
        Resolved                // All MatchCompleted handlers completed, safe to change scenes.
    }

    public struct MatchPlayerConfig {
        public NetworkConnection Connection;
        public short PlayerControllerId;
        public PlayerType Type;
        public PlayerSelection Selection;
    }

    public class MatchConfig {
        public MatchPlayerConfig[] PlayerSelections;
        public float Time = -1f;
        public int Stocks = -1;
    }

    /// <summary> 
    /// The Match Singleton. Manages the current state of the match and all of the defined Match rules. 
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Match")]
    public class Match : NetworkBehaviour {

        public static Match Current { get; private set; }

        public MatchConfig Config { get; private set; }
        public PlayerSet Players { get; private set; }
        public MatchStatus Status { get; private set; }

        Mediator _eventManager;
        MatchRule[] _rules;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Current = this;
            Players = new PlayerSet();
            _eventManager = Mediator.Global;
            _rules = GetComponents<MatchRule>();
            Status = MatchStatus.Initialization;
        }

        public ITask Initialize(MatchConfig config) {
            if (Status != MatchStatus.Initialization)
                throw new InvalidOperationException("Cannot initialize an already initialized Match");
            Config = config;
            foreach (var rule in _rules)
                rule.Initialize(Config);
            _rules = _rules.Where(r => r.IsActive).ToArray();
            Status = MatchStatus.Spawning;
            Assert.IsNotNull(Config);
            var spawnTasks = new List<ITask>();
            for (var i = 0; i < Config.PlayerSelections.Length; i++) {
                Debug.LogFormat("Spawning player {0}...", i + 1);
                var task = SpawnPlayer(Players.Get(i), Config.PlayerSelections[i]);
                spawnTasks.Add(task);
            }
            return Task.All(spawnTasks).Then(() => {
                Status = MatchStatus.Running;
                Debug.Log("Starting match...");
                _eventManager.Publish(new MatchStarted{ Match = this });
            });
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate() {
            if (!hasAuthority || Status != MatchStatus.Running)
                return;
            foreach (var rule in _rules)
                rule.OnMatchTick();
        }

        /// <summary>
        /// Finishes and resolves a match.
        /// </summary>
        /// <param name="result"> what is the result of the match </param>
        /// <param name="winner"> the winner of the match </param>
        public void Finish(MatchResult result, Player winner = null) {
            if (Status != MatchStatus.Running)
                throw new InvalidOperationException("Cannot finish a match not currently running.");
            Status = MatchStatus.Completed;
            _eventManager.PublishAsync(new MatchCompleted {
                Match = this,
                Result = result, 
                Winner = winner
            }).Then(() => {
                Debug.LogFormat("Match Completed, Result: {0}, Winner: {1}", result, winner);
                _eventManager.Publish(new MatchResolved { 
                    Match = this,
                    Result = result, 
                    Winner = winner
                });
            });
        }

        ITask SpawnPlayer(Player player, MatchPlayerConfig config) {
            // TODO(james7132): Split this large function into smaller parts.
            Assert.IsNotNull(player);
            var playerControllerId = config.PlayerControllerId;
            var conn = config.Connection;
            var selection = config.Selection;
            if (playerControllerId < conn.playerControllers.Count && 
                conn.playerControllers[playerControllerId].IsValid && 
                conn.playerControllers[playerControllerId].gameObject != null) {
                Debug.LogError("There is already a player at that playerControllerId for this connections.");
                return Task.Resolved;
            }

            var startPosition = SmashNetworkManager.Instance.GetStartPosition();
            var character = selection.Character;
            // Analytics.CustomEvent("characterSelected", new Dictionary<string, object> {
            //     { "character", character != null ? character.name : "Random" },
            //     { "color" , selection.Pallete },
            // });
            if (character == null) {
                Debug.Log("No character was specfied, randomly selecting character and pallete...");
                selection.Character = DataManager.Characters.Random();
                selection.Pallete = Mathf.FloorToInt(Random.value * selection.Character.PalleteCount);
            }
            var sameCharacterSelections = new HashSet<PlayerSelection>(Players.Select(p => p.Selection));
            if (sameCharacterSelections.Contains(selection)) {
                bool success = false;
                for (var i = 0; i < selection.Character.PalleteCount; i++) {
                    selection.Pallete = i;
                    if (!sameCharacterSelections.Contains(selection)) {
                        success = true;
                        break;
                    }
                }
                if (!success) {
                    Debug.LogError("Two players made the same selection, and no remaining palletes remain. {0} doesn't have enough colors", selection.Character);
                    ClientScene.RemovePlayer(playerControllerId);
                    return Task.Resolved;
                }
            }

            return selection.Character.Prefab.LoadAsync().Then(prefab => {
                if (prefab == null) {
                    Debug.LogError("The character {0} does not have a prefab. Please add a prefab object to it.", selection.Character);
                    return;
                }

                if (prefab.GetComponent<NetworkIdentity>() == null) {
                    Debug.LogError("The character prefab for {1} does not have a NetworkIdentity. Please add a NetworkIdentity to it's prefab.", selection.Character);
                    return;
                }

                GameObject playerObj;
                var startPos = startPosition != null ? startPosition.position : Vector3.zero;
                var startRot = startPosition != null ? startPosition.rotation : Quaternion.identity;
                playerObj = Instantiate(prefab, startPos, startRot);
                var characterComponent = playerObj.SafeGetComponent<Character>();
                characterComponent.State.Position = startPos;
                player.Selection = selection;
                player.Type = config.Type;
                player.PlayerObject = characterComponent;
                _eventManager.Publish(new PlayerSpawnEvent {
                    Player = player,
                    PlayerObject = playerObj
                });
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                NetworkServer.SendToAll(SmashNetworkMessages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
                // PlayerMap[playerConnection] = player;
                playerObj.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
            });
        }

    }

}
