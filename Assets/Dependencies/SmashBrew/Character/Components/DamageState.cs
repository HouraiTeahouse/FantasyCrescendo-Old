using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Damage State")]
    public class DamageState : CharacterComponent, IDamageable {

        public float DefaultDamage { get; set; }

        public ModifierGroup<object, float> DamageModifiers { get; private set; }
        public ModifierGroup<object, float> HealingModifiers { get; private set; }

        public DamageType Type { get; set; }

        protected override void Awake() {
            base.Awake();
            DamageModifiers = new ModifierGroup<object, float>();
            HealingModifiers = new ModifierGroup<object, float>();
            Type = DamageType.Percent;
        }

        internal float ModifyDamage(float baseDamage, object source = null) {
            return DamageModifiers.Out.Modifiy(source, baseDamage);
        }

        public override void ResetState(ref CharacterStateSummary state) { 
            state.Damage = DefaultDamage;
        }

        public override void ApplyState(ref CharacterStateSummary state) {
        }

        public void Damage(float damage) { Damage(null, damage); }

        public void Damage(object source, float damage) {
            damage = DamageModifiers.In.Modifiy(source, Mathf.Abs(damage));
            Character.State.Damage = Type.Damage(Character.State.Damage, damage);
        }

        public void Heal(float healing) { Heal(null, healing); }

        public void Heal(object source, float healing) {
            healing = HealingModifiers.In.Modifiy(source, Mathf.Abs(healing));
            Character.State.Damage = Type.Heal(Character.State.Damage, healing);
        }

    }

    public class DamageType {

        public static readonly DamageType Percent = new DamageType {
            _change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "%",
            Range = new Range(0, 999)
        };

        public static readonly DamageType Stamina = new DamageType {
            _change = (currentDamage, delta) => currentDamage + delta,
            Suffix = "HP",
            Range = new Range(0, 999)
        };

        Func<float, float, float> _change;

        DamageType() { }
        public string Suffix { get; private set; }
        public Range Range { get; private set; }

        public float Damage(float currentDamage, float delta) {
            return Range.Clamp(_change(currentDamage, Mathf.Abs(delta)));
        }

        public float Heal(float currentDamage, float delta) {
            return Range.Clamp(_change(currentDamage, -Mathf.Abs(delta)));
        }

    }

}

