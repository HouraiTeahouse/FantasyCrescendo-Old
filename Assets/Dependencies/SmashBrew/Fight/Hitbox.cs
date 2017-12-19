using System;
using System.Collections.Generic;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using Random = System.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    [DisallowMultipleComponent]
    public sealed class Hitbox : MonoBehaviour, IResettable {

        static bool _initialized = false;

        static readonly Table2D<HitboxType, Action<Hitbox, Hitbox>> ReactionMatrix;
        static readonly List<Hitbox> _hitboxes;

        public static IEnumerable<Hitbox> ActiveHitboxes {
            get { return _hitboxes; }
        }

        //TODO: Add triggers for on hit effects and SFX
        //ParticleSystem _effect;
        //AudioSource _soundEffect;
        Collider[] _colliders;

        HashSet<object> _history;

        [SerializeField]
        HitboxType _type;
        public int Priority = 100;
        public float Damage = 5f;

        [Range(0, 360)]
        public float Angle = 45f;
        public float BaseKnockback;
        public float KnockbackScaling = 1f;
        public bool Reflectable;
        public bool Absorbable;

        [SerializeField]
        bool _absorbable;

        public HitboxType CurrentType {
            get { return _type; }
            set {
                _type = value;
                gameObject.tag = Config.Tags.HitboxTag;
                switch (value) {
                    case HitboxType.Damageable:
                    case HitboxType.Shield:
                        gameObject.layer = Config.Tags.HurtboxLayer;
                        break;
                    default:
                        gameObject.layer = Config.Tags.HitboxLayer;
                        break;
                }
            }
        }
        
        public bool IsActive {
            get { return CurrentType != HitboxType.Inactive; }
        }

        public float BaseDamage {
            get { return Source == null ? Damage : Source.GetComponent<DamageState>().ModifyDamage(Damage); }
        }

        public bool FlipDirection {
            get {
                //TODO: Implement properly
                return false;
            }
        }

        /// <summary> Whether hitboxes should be drawn or not. </summary>
        public static bool DrawHitboxes { get; set; }

        // Represents the source Character that owns this Hitbox
        // If this is a Offensive type hitbox, this ensures that the Character doesn't damage themselves
        // If this is a Damageable type Hitbox (AKA a Hurtbox) this is the character that the damage and knockback is applied to.
        public Character Source { get; set; }

        public IDamageable Damageable { get; private set; }
        public IKnockbackable Knockbackable { get; private set; }

        static Hitbox() {
            ReactionMatrix = new Table2D<HitboxType, Action<Hitbox, Hitbox>>();
            _hitboxes = new List<Hitbox>();
            ReactionMatrix[HitboxType.Offensive, HitboxType.Damageable] = delegate(Hitbox src, Hitbox dst) {
                if (dst.Damageable != null)
                    dst.Damageable.Damage(src, src.BaseDamage);
                if (dst.Knockbackable != null) {
                    var angle = Mathf.Deg2Rad * src.Angle;
                    dst.Knockbackable.Knockback(src, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
                }
                DrawEffect(src, dst);
            };
            ReactionMatrix[HitboxType.Offensive, HitboxType.Absorb] = 
                ExecuteInterface<IAbsorbable>(h => h.Absorbable, (a, o) => a.Absorb(o));
            ReactionMatrix[HitboxType.Offensive, HitboxType.Reflective] = 
                ExecuteInterface<IReflectable>(h => h.Reflectable, (a, o) => a.Reflect(o));
            ReactionMatrix[HitboxType.Offensive, HitboxType.Invincible] = DrawEffect;
        }

        static Action<Hitbox, Hitbox> ExecuteInterface<T>(Predicate<Hitbox> check, Action<T, object> action) {
            return delegate(Hitbox src, Hitbox dst) {
                if (!check(src))
                    return;
                foreach (T component in src.GetComponents<T>())
                    action(component, dst);
            };
        }

       static void DrawEffect(Hitbox src, Hitbox dst) { 
            // TODO(james7132): Implement
        }

        public static void Resolve(Hitbox src, Hitbox dst) {
            ReactionMatrix[src.CurrentType, dst.CurrentType](src, dst);
        }

        void Initialize() {
            if (_initialized)
                return;
            // Draw Hitboxes in Debug builds
            DrawHitboxes = Debug.isDebugBuild;
            _initialized = true;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Initialize();
            Source = GetComponentInParent<Character>();
            Damageable = GetComponentInParent<IDamageable>();
            Knockbackable = GetComponentInParent<IKnockbackable>();
            _history = new HashSet<object>();

            _colliders = GetComponents<Collider>();
            foreach (Collider col in _colliders)
                col.isTrigger = true;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            _hitboxes.Add(this); 
            _history.Clear();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            _hitboxes.Remove(this); 
            _history.Clear();
        }

#if UNITY_EDITOR
        bool gizmoInitialized;

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos() {
            if (!isActiveAndEnabled)
                return;
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !gizmoInitialized) {
                ResetState();
                gizmoInitialized = true;
            }
            if (IsActive)
                Gizmo.DrawColliders(GetComponents<Collider>(), Config.Debug.GetHitboxColor(CurrentType));
        }
#endif

        public bool CheckHistory(object obj) {
            var result = _history.Contains(obj);
            if (!result)
                _history.Add(obj);
            if (Source != null) {
                var charHistory = Source.CheckHistory(obj);
                return result || charHistory;
            }
            return result;
        }

        public void ClearHistory() {
            if (_history != null)
                _history.Clear();
        }

        public void DrawHitbox() {
            #if UNITY_EDITOR
            if (!DrawHitboxes && EditorApplication.isPlayingOrWillChangePlaymode)
            #else
            if (!DrawHitboxes)
            #endif
                return;
            if (CurrentType == HitboxType.Inactive)
                return;
            if (_colliders == null)
                _colliders = GetComponentsInChildren<Collider>();
            Color color = Config.Debug.GetHitboxColor(CurrentType);
            #if UNITY_EDITOR
            foreach (Collider col in GetComponentsInChildren<Collider>())
            #else
            foreach (Collider col in _colliders)
            #endif
                DrawCollider(col, color);
        }

        void DrawCollider(Collider col, Color color) {
            if (col == null)
                return;
            Mesh mesh = null;
            if (col is SphereCollider)
                mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);
            else if (col is BoxCollider)
                mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube);
            else if (col is CapsuleCollider)
                mesh = PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Capsule);
            else if (col is MeshCollider)
                mesh = ((MeshCollider) col).sharedMesh;
            if (mesh == null)
                return;
            var material = Config.Debug.HitboxMaterial;
            material.SetColor("_Color", color);
            material.SetPass(0);
            Graphics.DrawMeshNow(mesh, Gizmo.GetColliderMatrix(col));
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other) { 
            if (!enabled || !other.CompareTag(Config.Tags.HitboxTag))
                return;
            var otherHitbox = other.GetComponent<Hitbox>();
            if (otherHitbox == null || !ReactionMatrix.ContainsKey(CurrentType, otherHitbox.CurrentType))
                return;
            if (CheckHistory(other) || CheckHistory(otherHitbox) || CheckHistory(otherHitbox.Damageable) || CheckHistory(otherHitbox.Knockbackable))
                return;
            HitboxResolver.AddCollision(this, otherHitbox);
        }

        public bool ResetState() {
            bool val = CurrentType != CurrentType;
            CurrentType = CurrentType;
            ClearHistory();
            return val;
        }

        void IResettable.OnReset() {
            ResetState();
        }

    }

}
