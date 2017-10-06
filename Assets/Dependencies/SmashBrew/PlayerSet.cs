using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew {

    public class PlayerSet : IEnumerable<Player> {

        Player[] _players;

        public Player Get(int id) {
            if(!Check.Range(id, _players))
                throw new ArgumentOutOfRangeException();
            Assert.IsTrue(id < GameMode.Current.MaxPlayers);
            return _players[id];
        }

        public IEnumerator<Player> GetEnumerator() {
            return _players.Cast<Player>().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public int Count {
            get { return _players.Length; }
        }

        public PlayerSet() {
            // Note: These objects are not intended to be destroyed and thus do not unresgister these event handlers
            // If there is a need to remove or replace them, this will need to be changed.
            _players = new Player[0];
            RebuildPlayerArray();
        }

        void RebuildPlayerArray() {
            var maxPlayers= !GameMode.All.Any() ? 0 : GameMode.All.Max(mode => mode.MaxPlayers);
            if (maxPlayers <= Count)
                return;
            var temp = new Player[maxPlayers];
            if(_players != null)
                Array.Copy(_players, temp , _players.Length);
            for (var i = 0; i < temp.Length; i++) {
                if(temp[i] == null)
                    temp[i] = new Player(i);
            }
            _players = temp;
        }

        public void ResetAll() {
            var blankSelection = new PlayerSelection();
            foreach (Player player in _players) {
                if (player == null)
                    continue;
                player.Selection = blankSelection;
                player.Type = PlayerType.None;
            }
        }

    }

}
