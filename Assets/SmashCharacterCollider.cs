using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew {

    public class SmashCharacterCollider : MonoBehaviour {

        [SerializeField]
        Diamond _collider;

        Diamond _prevFrame;
        Diamond _prediction;
        Diamond _currentFrame;

        [SerializeField]
        Vector2 _movement;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            _prevFrame = TransformCollider(_collider);
            Vector3 diff = _movement * Time.deltaTime;
            _currentFrame = _prevFrame.PhysicsCast(diff);
            transform.position += (Vector3)(_currentFrame.Center - _prevFrame.Center);
            _prediction = _currentFrame.PhysicsCast(diff);
        }

        Diamond TransformCollider(Diamond collider) {
            collider.Center = transform.TransformPoint(collider.Center);
            return collider;
        }

        public void Move(Vector2 direction) {
            _movement += direction;
        }

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos() {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode) {
                _currentFrame.DrawConnects(_prevFrame, Color.yellow);
                _currentFrame.DrawConnects(_prediction, Color.grey);
                _prevFrame.Draw(Color.red);
                _prediction.Draw(Color.blue);
                _currentFrame.Draw(Color.green);
            } else {
                TransformCollider(_collider).Draw(Color.green);
            }
#endif
        }

    }

}
