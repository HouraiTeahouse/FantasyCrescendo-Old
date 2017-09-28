using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public abstract class NetworkEvent {
        public SmashNetworkManager NetworkManager; 
    }

    public class NetworkClientConnected : NetworkEvent {
        public NetworkConnection Connection;
    }

    public class NetworkClientDisconnected : NetworkEvent {
        public NetworkConnection Connection;
    }

    public class NetworkClientStarted : NetworkEvent {
        public NetworkClient Client;
    }

    public class NetworkClientStopped : NetworkEvent {
    }

    public class NetworkServerAddedPlayer : NetworkEvent {
        public NetworkConnection Connection;
        public short PlayerID;
        public PlayerSelection Selection;
    }

    public class NetworkServerRemovedPlayer : NetworkEvent {
        public NetworkConnection Connection;
        public PlayerController PlayerController;
    }

    public class NetworkServerStarted : NetworkEvent {
    }

    public class NetworkServerReady : NetworkEvent {
        public NetworkConnection Connection;
    }

    public class NetworkServerDisconnected : NetworkEvent {
        public NetworkConnection Connection;
    }

}

