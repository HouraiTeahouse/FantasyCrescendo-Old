using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public struct HitboxCollision {

        public Hitbox Source { get; set; }
        public Hitbox Destination { get; set; }

        public void Resolve() { Hitbox.Resolve(Source, Destination); }

    }

    /// <summary> The global resolver of hitbox collisions. </summary>
    public sealed class HitboxResolver : MonoBehaviour {

        /// <summary> 
        /// A global list of collisions since the last resolution, sorted by joint priority 
        /// </summary>
        static readonly List<HitboxCollision> _queuedCollisions;

        [SerializeField]
        [Tooltip("How often to resolve hitbox collsions, in seconds")]
        float _frequency = 1 / 60f;

        static Dictionary<IStrikable, HitboxCollision> _targetedCollisions;

        static HitboxResolver() {
            _queuedCollisions = new List<HitboxCollision>();
            _targetedCollisions = new Dictionary<IStrikable, HitboxCollision>();
        }

        /// <summary> 
        /// The last time hitbox collisions were resolved 
        /// </summary>
        float _timer;

        /// <summary> Registers a new collision for resoluiton. </summary>
        /// <param name="src"> the source hitbox </param>
        /// <param name="dst"> the target hitbox </param>
        /// <exception cref="ArgumentNullException"> <paramref name="src" /> or <paramref name="dst" /> are null </exception>
        public static void AddCollision(Hitbox src, Hitbox dst) {
            Argument.NotNull(src);
            Argument.NotNull(dst);
            // The priority on the collision is the product of the priority on both hitboxes and their 
            _queuedCollisions.Add(new HitboxCollision {
                Destination = dst, 
                Source = src
            });
        }

        public static IEnumerable<HitboxCollision> ResolveQueuedCollisions() {
            foreach (var collision in ResolveCollisions(_queuedCollisions))
                yield return collision;
            _queuedCollisions.Clear();
        }

        public static IEnumerable<HitboxCollision> ResolveCollisions(IEnumerable<HitboxCollision> rawCollisions) {
            _targetedCollisions.Clear();
            foreach (HitboxCollision collision in _queuedCollisions.OrderByDescending(CollisionPriority)) {
                if (AddStrikable(collision.Destination.Damageable, collision))
                    continue;
                AddStrikable(collision.Destination.Knockbackable, collision);
            }
            foreach (var collision in _targetedCollisions.Values) {
                yield return collision;
            }
        }

        static bool AddStrikable(IStrikable strikable, HitboxCollision collision) {
            if (strikable == null)
                return false;
            _targetedCollisions[strikable] = collision;
            return true;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            _timer = Time.realtimeSinceStartup;
            _targetedCollisions = new Dictionary<IStrikable, HitboxCollision>();
        }

        static int CollisionPriority(HitboxCollision collision) {
            var src = collision.Source;
            var dst = collision.Destination;
            return (int) src.CurrentType * (int) dst.CurrentType * src.Priority * dst.Priority;
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate() {
            float currentTime = Time.realtimeSinceStartup;
            float deltaTime = currentTime - _timer;
            if (deltaTime < _frequency)
                return;
            _timer = currentTime - deltaTime % _frequency;
            if (_queuedCollisions.Count <= 0)
                return;
            foreach (HitboxCollision collision in ResolveQueuedCollisions()) {
                collision.Resolve();
            }
        }

    }

}