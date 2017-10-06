using HouraiTeahouse.SmashBrew.States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class CharacterState : State<CharacterStateContext> {

        public string Name { get; private set; }
        public CharacterStateData Data { get; private set; }
        public string AnimatorName {
            get { return Name.Replace(".", "-"); }
        }
        public int AnimatorHash {
            get { return Animator.StringToHash(AnimatorName); }
        }

        public CharacterState(string name,
                              CharacterStateData data) {
            Name = name;
            Data = Argument.NotNull(data);
        }

        public CharacterState AddTransitionTo(CharacterState state, 
                                              Func<CharacterStateContext, bool> extraCheck = null) {
            if (extraCheck != null)
                AddTransition(ctx => ctx.NormalizedAnimationTime >= 1.0f && extraCheck(ctx) ? state : null);
            else
                AddTransition(ctx => ctx.NormalizedAnimationTime >= 1.0f ? state : null);
            return this;
        }

        public override State<CharacterStateContext> Passthrough(CharacterStateContext context) {
            var altContext = context.Clone();
            altContext.NormalizedAnimationTime = float.PositiveInfinity;
            return EvaluateTransitions(altContext);
        }

        public override StateEntryPolicy GetEntryPolicy (CharacterStateContext context) {
            return Data.EntryPolicy;
        }

        public static bool operator ==(CharacterState lhs, CharacterState rhs) {
            if (object.ReferenceEquals(lhs, null) && object.ReferenceEquals(rhs, null))
                return true;
            if (object.ReferenceEquals(lhs, null) ^ object.ReferenceEquals(rhs, null))
                return false;
            return lhs.AnimatorHash == rhs.AnimatorHash;
        }

        public static bool operator !=(CharacterState lhs, CharacterState rhs) {
            return !(lhs == rhs);
        }

    }

    public static class CharacterStateExtensions {

        public static IEnumerable<CharacterState> AddTransitionTo(this IEnumerable<CharacterState> states,
                                                                State<CharacterStateContext> state) {
            foreach (CharacterState characterState in states)
                characterState.AddTransition(ctx => ctx.NormalizedAnimationTime >= 1.0f ? state : null);
            return states;
        }

        public static void Chain(this IEnumerable<CharacterState> states) {
            CharacterState last = null;
            foreach (CharacterState state in states) {
                if (state == null)
                    continue;
                if (last != null)
                    last.AddTransitionTo(state);
                last = state;
            }
        }

    }

}

