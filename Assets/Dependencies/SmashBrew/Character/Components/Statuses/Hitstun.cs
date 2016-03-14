using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     A Status effect that causes Characters to become uncontrollable for a short period
    ///     after being hit
    /// </summary>
    [DisallowMultipleComponent]
    [RequiredCharacterComponent]
    [RequireComponent(typeof (Rigidbody))]
    public class Hitstun : Status {
        private Vector3 _oldVelocity;

        /// <summary>
        ///     Gets whether the player has been hit recently
        /// </summary>
        public bool IsHit {
            get { return enabled; }
        }

        /// <summary>
        ///     <see cref="Status.OnStatusUpdate" />
        /// </summary>
        protected override void OnStatusUpdate(float dt) {
            _oldVelocity = Rigidbody.velocity;
        }

        /// <summary>
        ///     Unity callback. Called on entry into a physical collision with another object.
        /// </summary>
        /// <param name="col">the collision data</param>
        protected virtual void OnCollisionEnter(Collision col) {
            if (!IsHit)
                return;

            var points = col.contacts;
            if (points.Length <= 0)
                return;

            var point = points[0].point;
            var normal = points[0].normal;
            var reflection = _oldVelocity - 2 * Vector2.Dot(_oldVelocity, normal) * normal;
            Debug.DrawRay(point, reflection, Color.green);
            Debug.DrawRay(point, normal, Color.red);
            Debug.DrawRay(point, -_oldVelocity, Color.yellow);
            Rigidbody.velocity = Vector3.ClampMagnitude(reflection, 0.8f * _oldVelocity.magnitude);
        }
    }
}