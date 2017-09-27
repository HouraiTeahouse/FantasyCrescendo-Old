using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary> 
    /// A Match Rule defining a Stock-based Match. AllPlayers will have a fixed number of lives to lose, via exiting
    /// the blast zone. After which they will no longer respawn, and cannot further participate. The winner is the last player
    /// standing. 
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Stock Match")]
    public sealed class StockMatch : MatchRule {

        Mediator _eventManager;
        int startStocks;

        /// <summary> 
        /// The store of how many lives each player currently has. 
        /// </summary>
        [SerializeField]
        SyncListInt _stocks = new SyncListInt();

        /// <summary> 
        /// Readonly indexer for how many stocks each player has remaining. 
        /// </summary>
        /// <param name="player"> the Player in question </param>
        /// <returns> the number of remaining stocks they have </returns>
        public int this[Player player] => _stocks[player.ID];

        public event Action StockChanged;

        protected override bool CheckActive(MatchConfig config) => config.Stocks > 0;

        protected override void OnInitialize(MatchConfig config) {
            startStocks = config.Stocks;
            _stocks.Clear();
            foreach (var player in Match.Players) {
                _stocks.Add(-1);
                player.Changed += () => {
                    if (player.Type.IsActive && _stocks[player.ID] < 0)
                        _stocks[player.ID] = startStocks;
                };
            }
            _eventManager = Mediator.Global;
            _stocks.Callback+= (op, index) => StockChanged?.Invoke();
            var context = _eventManager.CreateUnityContext(this);
            context.Subscribe<PlayerSpawnEvent>(OnSpawn);
            context.Subscribe<PlayerDieEvent>(OnPlayerDie);
        }

        internal override void OnMatchTick() {
            var players = Match.Players;
            Player winner = null;
            MatchResult? result = MatchResult.Tie;
            for (var i = 0; i < _stocks.Count; i++) {
                if (_stocks[i] <= 0)
                    continue;
                if (winner == null) {
                    winner = players.Get(i);
                    result = MatchResult.HasWinner;
                } else {
                    winner = null;
                    result = null;
                }
            }
            if (result != null)
                Match.Finish(result.Value, winner);
        }

        bool RespawnCheck(Player character) {
            if (!IsActive || !Check.Range(character.ID, _stocks))
                return false;
            return _stocks[character.ID] > 1;
        }

        void OnPlayerDie(PlayerDieEvent eventArgs) {
            if (eventArgs.Revived || _stocks[eventArgs.Player.ID] - 1 < 0)
                return;
            _stocks[eventArgs.Player.ID]--;
            _eventManager.Publish(new PlayerRespawnEvent {Player = eventArgs.Player});
            eventArgs.Revived = true;
        }

        void OnSpawn(PlayerSpawnEvent eventArgs) {
            if (!IsActive)
                return;
            _stocks[eventArgs.Player.ID] = startStocks;
        }

    }

}
