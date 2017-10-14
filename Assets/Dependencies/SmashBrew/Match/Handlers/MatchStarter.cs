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

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            var client = SmashNetworkManager.Instance.StartHost();
            NetworkServer.dontListen = true;
            if (Match.Current == null)
                throw new InvalidOperationException("A match instance cannot be found!");
            if (MatchConfigBuilder.Instance == null)
                throw new InvalidOperationException("A MatchConfigBuilder instance cannot be found!");
            MatchConfig config = MatchConfigBuilder.Instance.Build();
            for (var i = 0; i < config.PlayerSelections.Length; i++) {
                var localPlayer = config.PlayerSelections[i];
                ClientScene.AddPlayer(client.connection, localPlayer.PlayerControllerId);
                config.PlayerSelections[i].Connection = NetworkServer.connections[0];
            }
            Match.Current.Initialize(config);
        }

    }

}
