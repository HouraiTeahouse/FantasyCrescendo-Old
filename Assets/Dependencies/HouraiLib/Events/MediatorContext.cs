using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    public class MediatorContext : IDisposable {

        protected Mediator Mediator { get; }
        Dictionary<Type, List<Delegate>> _subscriptions;

        internal MediatorContext(Mediator mediator) {
            Mediator = Argument.NotNull(mediator);
            _subscriptions = new Dictionary<Type, List<Delegate>>();
        }

        public virtual void Subscribe<T>(Mediator.Event<T> callback) {
            List<Delegate> typeSubs;
            if (!_subscriptions.TryGetValue(typeof(T), out typeSubs)) {
                typeSubs = new List<Delegate>();
                _subscriptions.Add(typeof(T), typeSubs);
            }
            typeSubs.Add(callback);
            Mediator.Subscribe<T>(callback);
        }

        public virtual void Dispose() {
            foreach(var kv in _subscriptions)
                foreach (var sub in kv.Value)
                    Mediator.Unsubscribe(kv.Key, sub);
        }

    }

    public sealed class UnityMeditatorContext : MediatorContext {

        readonly UnityEngine.Object obj;
        bool isDisposed = false;

        internal UnityMeditatorContext(Mediator mediator, UnityEngine.Object obj) 
            : base(mediator) {
            obj = Argument.NotNull(obj);
        }

        public override void Subscribe<T>(Mediator.Event<T> callback) {
            Mediator.Event<T> checkedCallback = (args) => {
                if (isDisposed)
                    return;
                if (obj != null)
                    callback?.Invoke(args);
                else
                    Dispose();
            };
            base.Subscribe(checkedCallback);
        }

        public override void Dispose() {
            base.Dispose();
            isDisposed = true;
        }

    }

    public static class MediatorContextExtensions  {

        public static MediatorContext CreateContext(this Mediator mediator) 
            => new MediatorContext(mediator);

        public static MediatorContext CreateUnityContext(this Mediator mediator, UnityEngine.Object obj) 
            => new UnityMeditatorContext(mediator, obj);

    }

}
