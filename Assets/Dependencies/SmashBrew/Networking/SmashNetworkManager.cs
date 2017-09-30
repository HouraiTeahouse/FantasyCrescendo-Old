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
                NetworkManager = this,
                Client = client
            });
            _clientStartedTask.Then(() => {
                Log.Info("Client initialized.");
            });
        }

        public override void OnClientDisconnect(NetworkConnection conn) {
            Mediator.Publish(new NetworkClientDisconnected {
                NetworkManager = this
            });
        }

        public override void OnClientConnect(NetworkConnection conn) {
            if (clientLoadedScene) 
                return;
            // Ready/AddPlayer is usually triggered by a scene load completing. 
            // If no scene was loaded, then Ready/AddPlayer it here instead.
            _clientStartedTask.Then(() => {
                Log.Debug("Client connecting...");
                if (!ClientScene.ready)
                    ClientScene.Ready(conn);
                Mediator.Publish(new NetworkClientConnected {
                    NetworkManager = this
                });
            });
        }

        public override void OnStartServer() {
            Mediator.Publish(new NetworkServerStarted {
                NetworkManager = this
            });
        }

        public override void OnServerReady(NetworkConnection conn) {
            NetworkServer.SetClientReady(conn);
            Mediator.Publish(new NetworkServerReady {
                NetworkManager = this
            });
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            Mediator.Publish(new NetworkServerAddedPlayer {
                NetworkManager = this,
                Connection = conn,
                PlayerID = playerControllerId
            });
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader) {
            Mediator.Publish(new NetworkServerAddedPlayer {
                NetworkManager = this,
                Connection = conn,
                PlayerID = playerControllerId
            });
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController) {
            base.OnServerRemovePlayer(conn, playerController);
            Mediator.Publish(new NetworkServerRemovedPlayer {
                NetworkManager = this,
                Connection = conn,
                PlayerController = playerController 
            });
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            Mediator.Publish(new NetworkServerDisconnected {
                NetworkManager = this,
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
