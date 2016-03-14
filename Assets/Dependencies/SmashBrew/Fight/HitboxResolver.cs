using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     The global resolver of hitbox collisions.
    /// </summary>
    public sealed class HitboxResolver : MonoBehaviour {
        /// <summary>
        ///     A global list of collisions since the last resolution, sorted by joint priority
        /// </summary>
        private static PriorityList<HitboxCollision> _collisions;

        [SerializeField, Tooltip("How often to resolve hitbox collsions, in seconds")] private readonly float _frequency
            = 1 /
              60f;

        private Dictionary<IStrikable, HitboxCollision> _targetedCollisions;

        /// <summary>
        ///     The last time hitbox collisions were resolved
        /// </summary>
        private float _timer;

        /// <summary>
        ///     Registers a new collision for resoluiton.
        /// </summary>
        /// <param name="src">the source hitbox</param>
        /// <param name="dst">the target hitbox</param>
        /// <exception cref="ArgumentNullException">thrown if <paramref name="src" /> or <paramref name="dst" /> are null</exception>
        public static void AddCollision(Hitbox src, Hitbox dst) {
            if (!src || !dst)
                throw new ArgumentNullException();
            // The priority on the collision is the product of the priority on both hitboxes and their 
            _collisions.Add(new HitboxCollision {Destination = dst, Source = src},
                (int) src.CurrentType * (int) dst.CurrentType * src.Priority * dst.Priority);
        }

        /// <summary>
        ///     Unity callback. Called on object instantiation.
        /// </summary>
        private void Awake() {
            if (_collisions == null)
                _collisions = new PriorityList<HitboxCollision>();
            _timer = Time.realtimeSinceStartup;
            _targetedCollisions = new Dictionary<IStrikable, HitboxCollision>();
        }

        /// <summary>
        ///     Unity callback. Called repeatedly on a fixed timestep.
        /// </summary>
        private void FixedUpdate() {
            var currentTime = Time.realtimeSinceStartup;
            var deltaTime = currentTime - _timer;
            if (deltaTime < _frequency)
                return;
            _timer = currentTime - deltaTime % _frequency;
            if (_collisions.Count <= 0)
                return;
            _targetedCollisions.Clear();
            foreach (var collision in _collisions) {
                if (collision.Destination.Damageable == collision.Destination.Knockbackable)
                    AddStrikable(collision.Destination.Damageable, collision);
                else {
                    AddStrikable(collision.Destination.Damageable, collision);
                    AddStrikable(collision.Destination.Knockbackable, collision);
                }
            }
            _collisions.Clear();
            foreach (var collision in _targetedCollisions.Values)
                collision.Resolve();
        }

        private void AddStrikable(IStrikable strikable, HitboxCollision collision) {
            if (strikable == null)
                return;
            if (_targetedCollisions.ContainsKey(strikable))
                _targetedCollisions.Add(strikable, collision);
        }

        public struct HitboxCollision {
            public Hitbox Source { get; set; }
            public Hitbox Destination { get; set; }

            public void Resolve() {
                Hitbox.Resolve(Source, Destination);
            }
        }
    }
}