using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    public abstract class Status : HouraiBehaviour {
        private float _duration = Mathf.Infinity;

        public float EllapsedTime { get; private set; }

        public float Duration {
            get { return _duration; }
            set {
                _duration = value;
                enabled = EllapsedTime < Duration;
            }
        }

        public static T Apply<T>(GameObject target, float duration = -1f) where T : Status {
            if (!target)
                throw new ArgumentNullException("target");
            var instance = target.GetComponent<T>();
            if (instance == null)
                instance = target.AddComponent<T>();
            instance.StartStatus(duration);
            return instance;
        }

        public static T Apply<T>(Component target, float duration = -1f) where T : Status {
            if (!target)
                throw new ArgumentNullException("target");
            return Apply<T>(target.gameObject, duration);
        }

        protected virtual void Start() {
            enabled = false;
        }

        protected virtual float GetDeltaTime() {
            return DeltaTime;
        }

        private void Update() {
            var dt = GetDeltaTime();
            EllapsedTime += dt;
            OnStatusUpdate(dt);
            enabled = EllapsedTime < Duration;
        }

        private void OnEnable() {
            EllapsedTime = 0f;
            OnStatusStart();
        }

        private void OnDisable() {
            OnStatusEnd();
        }

        public void StartStatus(float duration = -1) {
            if (duration > 0)
                Duration = duration;
            enabled = true;
        }

        protected virtual void OnStatusStart() {
        }

        protected virtual void OnStatusUpdate(float dt) {
        }

        protected virtual void OnStatusEnd() {
        }
    }
}