using System;
using UnityConstants;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HouraiTeahouse.SmashBrew {
    [DisallowMultipleComponent]
    [RequireComponent(typeof (Collider))]
    public sealed class Hitbox : MonoBehaviour {
        public enum Type {
            // The values here are used as priority mulitpliers
            Offensive = 1,
            Damageable = 2,
            Invincible = 3,
            Intangible = 4,
            Shield = 10000,
            Absorb = 20000,
            Reflective = 30000
        }

        private static readonly Table2D<Type, Action<Hitbox, Hitbox>> ReactionMatrix;

        [SerializeField] [HideInInspector] private Mesh _capsule;

        //TODO: Add triggers for on hit effects and SFX
        //private ParticleSystem _effect;
        //private AudioSource _soundEffect;
        private Collider[] _colliders;

        [SerializeField] [HideInInspector] private Mesh _cube;

        [SerializeField] [HideInInspector] private Material _material;

        [SerializeField] [HideInInspector] private Mesh _sphere;

        static Hitbox() {
            ReactionMatrix = new Table2D<Type, Action<Hitbox, Hitbox>>();
            ReactionMatrix[Type.Offensive, Type.Damageable] = delegate(Hitbox src, Hitbox dst) {
                if (dst.Damageable != null)
                    dst.Damageable.Damage(src, src.BaseDamage);
                if (dst.Knockbackable != null)
                    //TODO : FIX
                    dst.Knockbackable.Knockback(src, Vector2.one);
                DrawEffect(src, dst);
            };
            ReactionMatrix[Type.Offensive, Type.Absorb] =
                delegate(Hitbox src, Hitbox dst) {
                    ExecuteInterface<IAbsorbable>(Type.Absorb, src, dst, h => h.Absorbable, (a, o) => a.Absorb(o));
                };
            ReactionMatrix[Type.Offensive, Type.Reflective] =
                delegate(Hitbox src, Hitbox dst) {
                    ExecuteInterface<IReflectable>(Type.Reflective, src, dst, h => h.Reflectable, (r, o) => r.Reflect(o));
                };
            ReactionMatrix[Type.Offensive, Type.Invincible] = DrawEffect;
        }

        /// <summary>
        ///     Whether hitboxes should be drawn or not.
        /// </summary>
        public static bool DrawHitboxes { get; set; }

        // Represents the source Character that owns this Hitbox
        // If this is a Offensive type hitbox, this ensures that the Character doesn't damage themselves
        // If this is a Damageable type Hitbox (AKA a Hurtbox) this is the character that the damage and knockback is applied to.
        public Character Source { get; set; }

        public IDamageable Damageable { get; private set; }

        public IKnockbackable Knockbackable { get; private set; }

        private static void ExecuteInterface<T>(Type typeCheck, Hitbox src, Hitbox dst, Predicate<Hitbox> check,
            Action<T, object> action) {
            ;
            if (!check(src))
                return;
            foreach (var t in src.GetComponents<T>())
                action(t, dst);
        }

        private static void DrawEffect(Hitbox src, Hitbox dst) {
            throw new NotImplementedException();
        }

        public static void Resolve(Hitbox src, Hitbox dst) {
            ReactionMatrix[src.CurrentType, dst.CurrentType](src, dst);
        }

        #region Unity Callbacks

        /// <summary>
        ///     Unity callback. Called on object instantiation.
        /// </summary>
        private void Awake() {
            Source = GetComponentInParent<Character>();
            Damageable = GetComponentInParent<IDamageable>();
            Knockbackable = GetComponentInParent<IKnockbackable>();
            //_effect = GetComponent<ParticleSystem>();
            //_soundEffect = GetComponent<AudioSource>();

            gameObject.tag = Tags.Hitbox;
            switch (type) {
                case Type.Damageable:
                case Type.Shield:
                    gameObject.layer = Layers.Hurtbox;
                    break;
                default:
                    gameObject.layer = Layers.Hitbox;
                    break;
            }
            _colliders = GetComponents<Collider>();
            foreach (var col in _colliders)
                col.isTrigger = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            GizmoUtil.DrawColliders(GetComponents<Collider>(), Config.Instance.GetHitboxColor(type), true);
        }
#endif

        private void OnRenderObject() {
            if (!DrawHitboxes)
                return;
            if (_colliders == null)
                _colliders = GetComponents<Collider>();
            var color = Config.Instance.GetHitboxColor(type);
            foreach (var col in _colliders)
                DrawCollider(col, color);
            GL.wireframe = true;
            foreach (var col in _colliders)
                DrawCollider(col, Color.white);
            ;
            GL.wireframe = false;
        }

        private void DrawCollider(Collider col, Color color) {
            if (col == null)
                return;
            Mesh mesh = null;
            var boxCol = col as BoxCollider;
            var sphereCol = col as SphereCollider;
            var capsuleCol = col as CapsuleCollider;
            var position = Vector3.zero;
            var rotation = Quaternion.identity;
            var scale = Vector3.one;
            Matrix4x4 localToWorld;
            if (boxCol != null) {
                mesh = _cube;
                position = boxCol.center;
                scale = boxCol.size;
                localToWorld = transform.localToWorldMatrix;
            }
            else if (sphereCol != null) {
                mesh = _sphere;
                position = sphereCol.center;
                scale = sphereCol.radius * Vector3.one;
                localToWorld = Matrix4x4.TRS(transform.position, transform.rotation,
                    Vector3.one * transform.lossyScale.Max());
            }
            else if (capsuleCol != null) {
                mesh = _capsule;
                position = capsuleCol.center;
                scale = Vector3.one * capsuleCol.radius * 2;
                scale[capsuleCol.direction] = capsuleCol.height / 2;
                switch (capsuleCol.direction) {
                    case 1:
                        rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 2:
                        rotation = Quaternion.Euler(90, 0, 0);
                        break;
                }
                localToWorld = transform.localToWorldMatrix;
            }
            else {
                localToWorld = transform.localToWorldMatrix;
            }
            _material.SetColor("_Color", color);
            _material.SetPass(0);
            Graphics.DrawMeshNow(mesh, localToWorld * Matrix4x4.TRS(position, rotation, scale));
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag(Tags.Hitbox))
                return;
            var otherHitbox = other.GetComponent<Hitbox>();
            if (otherHitbox == null || !ReactionMatrix.ContainsKey(type, otherHitbox.type))
                return;
            HitboxResolver.AddCollision(this, otherHitbox);
        }

        #endregion

        #region Serializable Fields

        [SerializeField] private Type type;

        [SerializeField] private int _priority = 100;

        [SerializeField] private float _damage = 5f;

        [SerializeField] private float _angle = 45f;

        [SerializeField] private float _baseKnockback;

        [SerializeField] private float _knockbackScaling;

        [SerializeField] private bool _reflectable;

        [SerializeField] private bool _absorbable;

        #endregion

        #region Public Access Properties

        public Type CurrentType {
            get { return type; }
            set { type = value; }
        }

        public int Priority {
            get { return _priority; }
            set { _priority = value; }
        }

        public float Damage {
            get { return _damage; }
            set { _damage = value; }
        }

        public float Angle {
            get { return _angle; }
            set { _angle = value; }
        }

        public float BaseKnockback {
            get { return _baseKnockback; }
            set { _baseKnockback = value; }
        }

        public float Scaling {
            get { return _knockbackScaling; }
            set { _knockbackScaling = value; }
        }

        public bool Reflectable {
            get { return _reflectable; }
            set { _reflectable = value; }
        }

        public bool Absorbable {
            get { return _absorbable; }
            set { _absorbable = value; }
        }

        public float BaseDamage {
            get { return Source == null ? _damage : Source.ModifyDamage(_damage); }
        }

        public bool FlipDirection {
            get {
                //TODO: Implement properly
                return false;
            }
        }

        #endregion
    }
}