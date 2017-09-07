using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Stage {

    [AddComponentMenu("SmashBrew/Stage/Ledge")]
    [RequireComponent(typeof(NetworkIdentity))]
    public class StageLedge : MonoBehaviour {

        [SerializeField]
        bool _direction;

        public bool Occupied { get; private set; }

        void OnTriggerEnter(Collider collider) {
            if (!collider.CompareTag(Config.Tags.LedgeTag))
                return;
            var character = collider.GetComponentInParent<Character>();
            if (character == null || character.StateController.CurrentState == character.States.LedgeRelease)
                return;
            character.State.Direction = _direction;
            character.State.CurrentLedge = GetComponent<NetworkIdentity>();
        }

    }

}

