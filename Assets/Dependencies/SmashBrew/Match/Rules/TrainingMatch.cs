using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Matches {

    public class TrainingMatch : MatchRule {

        protected override bool CheckActive(MatchConfig config) => false;

        protected override void OnInitialize(MatchConfig config) {
            var eventManager = Mediator.Global;
            var context = eventManager.CreateUnityContext(this);
            // ALways respawn players
            context.Subscribe<PlayerDieEvent>(args => 
                eventManager.Publish(new PlayerRespawnEvent {
                    Player = args.Player
                }));
        }

    }

}
