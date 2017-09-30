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
        Initialization = 0,     // Initializing and setting up
        WaitingOnServer,        // Wait on server to initialize
        Spawning,               // Match is spawning characters, players do not currently have control.
        Running,                // Players currently have control of their characters.
        Completed               // Match completed. Waiting on resolution.
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

        static ILog _log = Log.GetLogger<Match>();

        public static Match Current { get; private set; }

        public MatchConfig Config { get; private set; }
        public PlayerSet Players { get; private set; }
        public MatchStatus Status { get; private set; }

        Mediator _eventManager;
        MatchRule[] _rules;
        ITask _initialization;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Current = this;
            Players = new PlayerSet();
            Status = MatchStatus.Initialization;
            _eventManager = Mediator.Global;
            _initialization = new Task();
            SetupNetworkEvents();
        }

        public void Initialize(MatchConfig config) {
            if (Status != MatchStatus.Initialization)
                throw new InvalidOperationException("Cannot initialize an already initialized Match");
            Config = config;
            _rules = GetComponents<MatchRule>();
            foreach (var rule in _rules)
                rule.Initialize(Config);
            _rules = _rules.Where(r => r.IsActive).ToArray();
            _initialization.Resolve();
            Status = MatchStatus.Spawning;
            Assert.IsNotNull(Config);
            for (var i = 0; i < Config.PlayerSelections.Length; i++) {
                Log.Info("Spawning player {0}...", i + 1);
                SpawnPlayer(Players.Get(i), Config.PlayerSelections[i]);
            }
            Status = MatchStatus.Running;
            _log.Info("Starting match...");
            _eventManager.Publish(new MatchStartEvent());
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            Log.Error("ENABLED");
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            Log.Error("DISABLED");
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

        public void Finish(MatchResult result, Player winner) {
            Status = MatchStatus.Completed;
            _eventManager.Publish(new MatchEndEvent{
                Result = result, 
                Winner = winner
            });
        }

        void SpawnPlayer(Player player, MatchPlayerConfig config) {
            Assert.IsNotNull(player);
            var playerControllerId = config.PlayerControllerId;
            var conn = config.Connection;
            var selection = config.Selection;
            if (playerControllerId < conn.playerControllers.Count && 
                conn.playerControllers[playerControllerId].IsValid && 
                conn.playerControllers[playerControllerId].gameObject != null) {
                Log.Error("There is already a player at that playerControllerId for this connections.");
                return;
            }

            var startPosition = SmashNetworkManager.Instance.GetStartPosition();
            var character = selection.Character;
            bool random = character == null;
            // Analytics.CustomEvent("characterSelected", new Dictionary<string, object> {
            //     { "character", character != null ? character.name : "Random" },
            //     { "color" , selection.Pallete },
            // });
            if (random) {
                Log.Info("No character was specfied, randomly selecting character and pallete...");
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
                    Log.Error("Two players made the same selection, and no remaining palletes remain. {0} doesn't have enough colors".With(selection.Character));
                    ClientScene.RemovePlayer(playerControllerId);
                    return;
                }
            }

            selection.Character.Prefab.LoadAsync().Then(prefab => {
                if (prefab == null) {
                    Log.Error("The character {0} does not have a prefab. Please add a prefab object to it.", selection.Character);
                    return;
                }

                if (prefab.GetComponent<NetworkIdentity>() == null) {
                    Log.Error(
                        "The character prefab for {0} does not have a NetworkIdentity. Please add a NetworkIdentity to it's prefab.",
                        selection.Character);
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
                Log.Warning("HELLO");
                _eventManager.Publish(new PlayerSpawnEvent {
                    Player = player,
                    PlayerObject = playerObj
                });
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                NetworkServer.SendToAll(SmashNetworkMessages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
                var playerConnection = new PlayerConnection {
                    ConnectionID = conn.connectionId,
                    PlayerControllerID = playerControllerId
                };
                // PlayerMap[playerConnection] = player;
                playerObj.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
            }).Done();
        }

        void SetupNetworkEvents() {
            var context = Mediator.Global.CreateUnityContext(this);
            // context.Subscribe<NetworkClientStarted>(args =>{
            //     DestroyLeftoverPlayers();
            //     args.Client.RegisterHandler(SmashNetworkMessages.UpdatePlayer,
            //         msg => {
            //             var update = msg.ReadMessage<UpdatePlayerMessage>();
            //             var player = MatchPlayers.Get(update.ID);
            //             update.UpdatePlayer(player);
            //         });
            // });
            // context.Subscribe<NetworkClientConnected>(args => {
            //     foreach (var player in Players.Where(p => p.Type.IsActive)) {
            //         ClientScene.AddPlayer(args.Connection, localPlayerCount, PlayerSelectionMessage.FromSelection(player.Selection));
            //         localPlayerCount++;
            //     }
            // });
        //     context.Subscribe<NetworkClientDisconnected>(args => {
        //         DestroyLeftoverPlayers();
        //     });
        //     context.Subscribe<NetworkClientStarted>(args => {
        //         DestroyLeftoverPlayers();
        //         NetworkServer.RegisterHandler(SmashNetworkMessages.PlayerInput,
        //             msg => {
        //                 // TODO(james7132): check for client authority
        //                 var message = msg.ReadMessage<PlayerInputSet>();
        //                 var player = Players.Get(message.PlayerId);
        //                 if (player == null || player.PlayerObject == null)
        //                     return;
        //                 player.PlayerObject.ServerRecieveInput(message);
        //             });
        //         // Update players when they change
        //         foreach (var player in Players) {
        //             player.Changed += () => {
        //                 if (Network.isServer) {
        //                     NetworkServer.SendToAll(SmashNetworkMessages.UpdatePlayer, UpdatePlayerMessage.FromPlayer(player));
        //                 }
        //             };
        //         }
        //     });
        //     context.Subscribe<NetworkServerReady>(args => {
        //         foreach (var player in MatchPlayers)
        //             args.Connection.Send(SmashNetworkMessages.UpdatePlayer, 
        //                                  UpdatePlayerMessage.FromPlayer(player));
        //     });
        //     context.Subscribe<NetworkServerDisconnected>(args => {
        //         foreach (var playerController in args.Connection.playerControllers)
        //             RemovePlayer(args.Connection, playerController);
        //     });
        //     context.Subscribe<NetworkServerAddedPlayer>(AddPlayer);
        //     context.Subscribe<NetworkServerRemovedPlayer>(args => RemovePlayer(args.Connection, args.PlayerController));
        //     context.Subscribe<NetworkClientStopped>(args => {
        //         playerCount = 0;
        //         Players.ResetAll();
        //     });
        // }

        // void RemovePlayer(NetworkConnection conn, PlayerController controller) {
        //     var playerConnection = new PlayerConnection {
        //         ConnectionID = conn.connectionId,
        //         PlayerControllerID = controller.playerControllerId
        //     };
        //     if(PlayerMap.ContainsKey(playerConnection)) {
        //         var player = PlayerMap[playerConnection];
        //         player.Type = PlayerType.None;
        //         player.Selection = new PlayerSelection();
        //         PlayerMap.Remove(playerConnection);
        //     }
        //     playerCount = Mathf.Max(0, playerCount - 1);
        }


        void DestroyLeftoverPlayers() {
            // localPlayerCount = 0;
            // playerCount = 0;
            // var players = Resources.FindObjectsOfTypeAll<Character>();
            // foreach (Character character in players) {
            //     if (character.gameObject.scene.isLoaded)
            //         Destroy(character.gameObject);
            // }
            // MatchPlayers.ResetAll();
        }

    }

}
