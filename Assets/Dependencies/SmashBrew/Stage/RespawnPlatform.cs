using HouraiTeahouse.SmashBrew.Characters;
using HouraiTeahouse.SmashBrew.Characters.Statuses;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Stage {

    [AddComponentMenu("Smash Brew/Stage/Respawn")]
    public class RespawnPlatform : NetworkBehaviour {

        Character _character;
        Invincibility _invincibility;

        [SerializeField]
        bool _facing;

        [SerializeField]
        float _invicibilityTimer;

        [SerializeField]
        float _platformTimer;

        [SyncVar]
        float _timer;

        [SyncVar(hook = "OccupationChanged")]
        bool _isOccupied;

        public bool Occupied {
            get { return _isOccupied; }
            private set { OccupationChanged(value); }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            var context = Mediator.Global.CreateUnityContext(this);
            context.Subscribe<PlayerRespawnEvent>(OnEvent);
        }

        public override void OnStartServer() {
            gameObject.SetActive(false);
            _isOccupied = false;
        }

        public override void OnStartClient() {
            gameObject.SetActive(_isOccupied);
        }

        void OccupationChanged(bool isOccupied) {
            _isOccupied = isOccupied;
            gameObject.SetActive(_isOccupied);
        }

        void OnEvent(PlayerRespawnEvent eventArgs) {
            if (Occupied || eventArgs.Consumed)
                return;
            eventArgs.Consumed = true;
            //TODO(james7132): Fix this
            _character = eventArgs.Player.PlayerObject;
            _character.State.Position = transform.position;
            _character.ResetCharacter();
            _invincibility = Status.Apply<Invincibility>(_character, _invicibilityTimer + _platformTimer);
            _timer = 0f;
            _character.gameObject.SetActive(true);
            Occupied = true;
            _isOccupied = true;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (!isServer || _character == null)
                return;

            _timer += Time.deltaTime;

            // TODO: Find better alternative to this hack
            if (_timer > _platformTimer) {
                _invincibility.Duration -= _platformTimer;
                _character.ResetCharacter();

                Occupied = false;
                gameObject.SetActive(false);
            }
        }

    }

}
