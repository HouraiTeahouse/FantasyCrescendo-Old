using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [RequireComponent(typeof(KnockbackState))]
    public class HitState : CharacterNetworkComponent {

        /// <summary>
        /// The amount of hitstun time remaining in seconds.
        /// </summary>
        public float Hitstun { get; set; }

        KnockbackState KnockbackState { get; set; }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            KnockbackState = this.SafeGetComponent<KnockbackState>();
            if (KnockbackState == null)
                return;
            KnockbackState.OnHit += (src, dir) => {
                var hitbox = src as Hitbox;
                if (hitbox == null)
                    return;
                Hitstun = Config.Fight.CalculateHitstun(hitbox.Damage);
            };
        }

        public override void Simulate(float deltaTime, 
                                      ref CharacterStateSummary state,
                                      ref InputContext input) {
            if (state.Hitstun <=  0f)
                return;
            state.Hitstun = Mathf.Max(0f, Hitstun - Time.deltaTime);
        }

        public override void ResetState(ref CharacterStateSummary state) {
            state.Hitstun = 0f;
        }

        public override void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context) {
            context.IsHit = summary.Hitstun > 0f;
        }

    }

}
