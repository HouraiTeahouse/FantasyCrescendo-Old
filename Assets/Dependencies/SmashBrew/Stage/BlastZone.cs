using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Stage {

    /// <summary> The Blast Zone script. Publishes PlayerDieEvents in response to Players leaving it's bounds. </summary>
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Smash Brew/Stage/Blast Zone")]
    public sealed class BlastZone : NetworkBehaviour {

        Collider _col;
        Mediator _eventManager;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            _eventManager = Mediator.Global;
            _col = GetComponent<Collider>();
            // Make sure that the colliders are triggers
            foreach (Collider col in gameObject.GetComponents<Collider>())
                col.isTrigger = true;
        }

        /// <summary>
        /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerExit(Collider other) {
            // Filter only for player characters
            var character = other.GetComponentInParent<Character>();
            if (!isServer || character == null || character.Player == null)
                return;

            Vector3 position = other.transform.position;
            if (_col.ClosestPointOnBounds(position) == position)
                return;
           
            character.gameObject.SetActive(false);
            _eventManager.Publish(new PlayerDieEvent { 
                Player = character.Player, 
                Revived = false 
            });
        }

    }

}
