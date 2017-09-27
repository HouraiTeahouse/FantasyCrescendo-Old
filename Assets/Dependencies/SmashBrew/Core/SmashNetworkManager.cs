using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public struct PlayerConnection {
        public int ConnectionID;
        public int PlayerControllerID;
        public override int GetHashCode() { return ConnectionID * 37 + PlayerControllerID; }
    }

    public class SmashNetworkMessages {
        public const short UpdatePlayer = MsgType.Highest + 1;
        public const short PlayerInput = MsgType.Highest + 1;
    }

    public class NetworkClientConnected {
        public NetworkConnection Connection;
    }

    public class NetworkClientDisconnected {
        public NetworkConnection Connection;
    }

    public class NetworkClientStarted {
        public NetworkClient Client;
    }

    public class NetworkClientStopped {
    }

    public class NetworkServerAddedPlayer {
        public NetworkConnection Connection;
        public short PlayerID;
        public PlayerSelection Selection;
    }

    public class NetworkServerRemovedPlayer {
        public NetworkConnection Connection;
        public PlayerController PlayerController;
    }

    public class NetworkServerStarted {
    }

    public class NetworkServerReady {
        public NetworkConnection Connection;
    }

    public class NetworkServerDisconnected {
        public NetworkConnection Connection;
    }
    
    [RequireComponent(typeof(PlayerManager))]
    public class SmashNetworkManager : NetworkManager {

        public static SmashNetworkManager Instance { get; private set; }

        public Mediator Mediator { get; private set; }

        ITask _clientStartedTask;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Instance = this;
            Mediator = Mediator.Global;
        }

        public override void OnStartClient(NetworkClient client) {
            base.OnStartClient(client);
            Log.Info("Starting client initialization.");
            _clientStartedTask = Mediator.PublishAsync(new NetworkClientStarted {
                Client = client
            });
            _clientStartedTask.Then(() => {
                Log.Info("Client initialized.");
            });
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            Mediator.Publish(new NetworkClientDisconnected());
        }

        public override void OnClientConnect(NetworkConnection conn) {
            if (clientLoadedScene) 
                return;
            // Ready/AddPlayer is usually triggered by a scene load completing. 
            // If no scene was loaded, then Ready/AddPlayer it here instead.
            _clientStartedTask.Then(() => {
                Log.Debug("Client connecting...");
                ClientScene.Ready(conn);
                Mediator.Publish(new NetworkClientConnected());
            });
        }

        public override void OnStartServer() {
            Mediator.Publish(new NetworkServerStarted());
        }

        public override void OnServerReady(NetworkConnection conn) {
            NetworkServer.SetClientReady(conn);
            Mediator.Publish(new NetworkServerReady());
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            Mediator.Publish(new NetworkServerAddedPlayer {
                Connection = conn,
                PlayerID = playerControllerId
            });
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
            Mediator.Publish(new NetworkServerAddedPlayer {
                Connection = conn,
                PlayerID = playerControllerId
            });
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController) {
            base.OnServerRemovePlayer(conn, playerController);
            Mediator.Publish(new NetworkServerRemovedPlayer {
                Connection = conn,
                PlayerController = playerController 
            });
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            Mediator.Publish(new NetworkServerDisconnected {
                Connection = conn
            });
            base.OnServerDisconnect(conn);
        }

        public override void OnStopClient() {
            base.OnStopClient();
            Mediator.Publish(new NetworkClientStopped());
        }

    }

}
