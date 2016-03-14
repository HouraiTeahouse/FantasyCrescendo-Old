using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     A Status effect that prevents players from taking damage while active.
    /// </summary>
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    public sealed class Invincibility : Status {
        private Character _character;
        private Hitbox[] _hitboxes;

        /// <summary>
        ///     Unity callback. Called once before the object's first frame.
        /// </summary>
        protected override void Start() {
            base.Start();
            _character = GetComponent<Character>();
            _character.DamageModifiers.In.Add(InvincibilityModifier, int.MaxValue);
            _hitboxes = GetComponentsInChildren<Hitbox>();
        }

        /// <summary>
        ///     Unity callback. Called on object destruction.
        /// </summary>
        private void OnDestroy() {
            if (_character)
                _character.DamageModifiers.In.Remove(InvincibilityModifier);
        }

        /// <summary>
        ///     Unity callback. Called when component is enabled.
        /// </summary>
        private void OnEnable() {
            if (_hitboxes == null)
                _hitboxes = GetComponentsInChildren<Hitbox>();
            foreach (var hitbox in _hitboxes)
                if (hitbox.CurrentType == Hitbox.Type.Damageable)
                    hitbox.CurrentType = Hitbox.Type.Invincible;
            ;
        }

        /// <summary>
        ///     Unity callback. Called when component is disabled.
        /// </summary>
        private void OnDisable() {
            if (_hitboxes == null)
                _hitboxes = GetComponentsInChildren<Hitbox>();
            foreach (var hitbox in _hitboxes)
                if (hitbox.CurrentType == Hitbox.Type.Invincible)
                    hitbox.CurrentType = Hitbox.Type.Damageable;
        }

        /// <summary>
        ///     Invincibilty modifier. Negates all damage while active.
        /// </summary>
        private float InvincibilityModifier(object source, float damage) {
            return enabled ? damage : 0f;
        }
    }
}