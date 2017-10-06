using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A UI Text driver that displays the current damage of the a given player. </summary>
    public sealed class PlayerDamageDisplay : GradientNumberText, IDataComponent<Player> {

        Player _player;
        DamageState _damage;

        [SerializeField]
        [Tooltip("The font size of the suffix")]
        int suffixSize = 25;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Mediator.Global.CreateUnityContext(this)
                    .Subscribe<PlayerChanged>(args =>{
                        if (args.Player == _player)
                            PlayerChange();
                    });
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            _player = data;
            PlayerChange();
        }

        void PlayerChange() {
            if (_player == null || _player.PlayerObject == null)
                _damage = null;
            else {
                _damage = _player.PlayerObject.GetComponentInChildren<DamageState>();
                Number = _damage.Character.State.Damage;
            }
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected override void Update() {
            base.Update();
            if (!Text || !_damage)
                return;
            //TODO: Change this into a event
            bool visible = _damage.isActiveAndEnabled;
            Text.enabled = visible;
            float value = Mathf.Floor(_damage.Character.State.Damage);
            if (visible && !Mathf.Approximately(Number, value))
                Number = value;
        }

        /// <summary>
        ///     <see cref="NumberText.ProcessNumber" />
        /// </summary>
        protected override string ProcessNumber(string number) {
            if (!_damage)
                return number;
            return string.Format("{0}<size={1}>{2}</size>", number, suffixSize, _damage.Type.Suffix);
        }

    }

}
