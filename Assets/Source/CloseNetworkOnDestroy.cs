using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

    /// <summary>
    /// A cleanup script that closes all network connections upon destruction.
    /// Useful for automatically terminating any matches when a scene is loaded.
    /// </summary>
    public class CloseNetworkOnDestroy : MonoBehaviour {

        NetworkManager _manager;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            _manager = FindObjectOfType<NetworkManager>();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            if (_manager == null)
                return;
            if (NetworkServer.active && _manager.IsClientConnected())
                _manager.StopHost();
            else if (NetworkServer.active)
                _manager.StopServer();
            else if (_manager.IsClientConnected())
                _manager.StopClient();
        }

    }

}
