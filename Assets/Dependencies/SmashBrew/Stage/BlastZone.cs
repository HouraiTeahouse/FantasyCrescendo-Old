using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     The Blast Zone script.
    ///     Publishes PlayerDieEvents in response to Players leaving it's bounds.
    /// </summary>
    [RequireComponent(typeof (Collider))]
    public sealed class BlastZone : MonoBehaviour {
        private Collider _col;
        private Mediator _eventManager;

        /// <summary>
        ///     Unity Callback. Called on object instantiation.
        /// </summary>
        private void Awake() {
            _eventManager = GlobalMediator.Instance;
            _col = GetComponent<Collider>();
            // Make sure that the colliders are triggers
            foreach (var col in gameObject.GetComponents<Collider>())
                col.isTrigger = true;
        }

        /// <summary>
        ///     Unity Callback. Called on Trigger Collider entry.
        /// </summary>
        /// <param name="other">the other collider that entered the c</param>
        private void OnTriggerExit(Collider other) {
            // Filter only for player characters
            var player = Player.GetPlayer(other);
            if (player == null)
                return;

            var position = other.transform.position;
            if (_col.ClosestPointOnBounds(position) == position)
                return;

            _eventManager.Publish(new PlayerDieEvent {Player = player, Revived = false});
        }
    }
}