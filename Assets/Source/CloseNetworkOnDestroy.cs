using HouraiTeahouse.SmashBrew;
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

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            var manager = SmashNetworkManager.Instance;
            if (manager == null)
                return;
            manager.StopHost();
            manager.StopServer();
            manager.StopClient();
            NetworkServer.Reset();
        }

    }

}
