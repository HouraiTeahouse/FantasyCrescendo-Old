using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Matches {

    public class PlayerStockChanged {
        public Player Player;
        public int Stocks;
    }

    /// <summary> 
    /// A Match Rule defining a Stock-based Match. AllPlayers will have a fixed number of lives to lose, via exiting
    /// the blast zone. After which they will no longer respawn, and cannot further participate. The winner is the last player
    /// standing. 
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Stock Match")]
    public sealed class StockMatch : MatchRule {

        int startStocks;

        protected override bool CheckActive(MatchConfig config) => config.Stocks > 0;

        protected override void OnInitialize(MatchConfig config) {
            startStocks = config.Stocks;
            var _eventManager = Mediator.Global;
            var context = _eventManager.CreateUnityContext(this);
            context.Subscribe<PlayerSpawnEvent>(args => {
                Assert.IsTrue(IsActive);
                args.Player.PlayerObject.State.Stocks = (byte)startStocks;
            });
            context.Subscribe<PlayerDieEvent>(args => {
                Assert.IsTrue(IsActive);
                var stocks = GetStock(args.Player);
                if (args.Revived || stocks <= 0)
                    return;
                var character = args.Player.PlayerObject;
                Assert.IsNotNull(character);
                character.State.Stocks--;
                _eventManager.Publish(new PlayerStockChanged {
                    Player = args.Player,
                    Stocks = character.State.Stocks
                });
                if (character.State.Stocks > 0)
                    _eventManager.Publish(new PlayerRespawnEvent {Player = args.Player});
                args.Revived = true;
                MatchFinishCheck();
            });
        }

        internal override void OnMatchTick() => MatchFinishCheck();

        int GetStock(Player player) => player?.PlayerObject?.State.Stocks ?? -1;

        void MatchFinishCheck() {
            var players = Match.Players;
            Player winner = null;
            MatchResult? result = MatchResult.Tie;
            foreach (var player in Match.Players) {
                var stockCount = GetStock(player);
                if (stockCount <= 0)
                    continue;
                if (winner == null) {
                    winner = player;
                    result = MatchResult.HasWinner;
                } else {
                    winner = null;
                    result = null;
                    break;
                }
            }
            if (result != null)
                Match.Finish(result.Value, winner);
        }

    }

}
