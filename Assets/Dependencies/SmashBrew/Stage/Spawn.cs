using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Matches;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Stage {

    /// <summary> A EventHandler for spawning characters at the start of the match </summary>
    [AddComponentMenu("Smash Brew/Stage/Spawn")]
    public class Spawn : EventBehaviour<MatchStarted> {

        [Serializable]
        class SpawnPoint {
#pragma warning disable 0649
            public Transform Point;
            public bool Direction;
#pragma warning restore 0649
        }

        [SerializeField]
        [Tooltip("The spawn points for each of the characters")]
        SpawnPoint[] _spawnPoints;

        /// <summary> Spawns players when the match begins. </summary>
        /// <param name="eventArgs"> </param>
        protected override void OnEvent(MatchStarted eventArgs) {
            //var i = 0;
            //var playerManager = PlayerManager.Instance;
            //IEnumerator<Player> activePlayers = playerManager.MatchPlayers.Where(p => p.Type.IsActive).GetEnumerator();
            //while (i < _spawnPoints.Length && activePlayers.MoveNext()) {
            //    Player player = activePlayers.Current;
            //    Character runtimeCharacter = player.Spawn(_spawnPoints[i].Point, _spawnPoints[i].Direction);
            //    i++;
            //    if (runtimeCharacter == null)
            //        continue;

            //    //TODO: Fix this hack, get netplay working
            //    runtimeCharacter.gameObject.SetActive(true);
            //    runtimeCharacter.name = "Player {0} ({1})".With(player.ID + 1, player.SpawnedCharacter.name);
            //    EventManager.Publish(new PlayerSpawnEvent {Player = player, PlayerObject = runtimeCharacter.gameObject});
            //}
        }

    }

}
