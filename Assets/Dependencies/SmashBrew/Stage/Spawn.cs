using System;
using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     A EventHandler for spawning characters at the start of the match
    /// </summary>
    public class Spawn : EventHandlerBehaviour<MatchStartEvent> {
        [SerializeField, Tooltip("The spawn points for each of the characters")] private SpawnPoint[] _spawnPoints;

        /// <summary>
        ///     Spawns players when the match begins.
        /// </summary>
        /// <param name="startEventArgs"></param>
        protected override void OnEvent(MatchStartEvent startEventArgs) {
            var i = 0;
            var activePlayers = Player.ActivePlayers.GetEnumerator();
            while (i < _spawnPoints.Length && activePlayers.MoveNext()) {
                var player = activePlayers.Current;
                var runtimeCharacter = player.Spawn(_spawnPoints[i].Point, _spawnPoints[i].Direction);
                i++;
                if (runtimeCharacter == null)
                    continue;

                //TODO: Fix this hack, get netplay working
                runtimeCharacter.gameObject.SetActive(true);
                runtimeCharacter.name = string.Format("Player {0} ({1})", player.PlayerNumber + 1,
                    player.SpawnedCharacter.name);
                EventManager.Publish(new PlayerSpawnEvent {Player = player, PlayerObject = runtimeCharacter.gameObject});
            }
        }

        [Serializable]
        private class SpawnPoint {
            public bool Direction;
            public Transform Point;
        }
    }
}