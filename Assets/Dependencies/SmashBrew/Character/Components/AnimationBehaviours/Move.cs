using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     An AnimationBehaviour that causes Characters to constantly move while in the state
    /// </summary>
    public class Move : BaseAnimationBehaviour<Character> {
        [SerializeField, Tooltip("The base movement speed that the character will move at while in the state")] private readonly float _baseSpeed = 3f;

        [SerializeField, Tooltip("Whether movement in the state ignores or adheres to the difference in state speed")] private bool _ignoreStateSpeed;

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (!Target)
                return;
            if (_ignoreStateSpeed)
                Target.Move(_baseSpeed);
            else
                Target.Move(_baseSpeed * stateInfo.speed);
        }
    }
}