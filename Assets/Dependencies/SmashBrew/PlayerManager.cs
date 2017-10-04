using HouraiTeahouse.SmashBrew.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerManager : MonoBehaviour {

        Player[] _matchPlayers;

        public static PlayerManager Instance { get; private set; }

        public PlayerSet Players { get; private set; }

        Dictionary<PlayerConnection, Player> PlayerMap;

        short localPlayerCount;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Instance = this;
            Config.Load();
            Players = new PlayerSet();
        }

    }

}
