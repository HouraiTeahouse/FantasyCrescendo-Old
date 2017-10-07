using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace HouraiTeahouse {

    /// <summary> 
    /// A generalized object that encapsulates the interactions between multiple objects. 
    /// Meant to be used as either local or global event managers. 
    /// </summary>
    public class Mediator {

        public delegate void Event<in T>(T arg);
        public delegate ITask AsyncEvent<in T>(T arg);

        static readonly ILog log = Log.GetLogger("Events");

        // Global event bus
        public static readonly Mediator Global = new Mediator();

        // Maps types of events to a set of handlers
        readonly Dictionary<Type, List<Delegate>> _subscribers;
        readonly Dictionary<Type, Type[]> _typeCache;

        /// <summary> 
        /// Initializes an instance of Mediator 
        /// </summary>
        public Mediator() {
            _subscribers = new Dictionary<Type, List<Delegate>>();
            _typeCache = new Dictionary<Type, Type[]>();
        }

        // Internal only, for testing
        internal Mediator(Dictionary<Type, List<Delegate>> subscribers) : this() {
            Argument.NotNull(subscribers);
            _subscribers = new Dictionary<Type, List<Delegate>>(subscribers);
        }

        /// <summary> 
        /// Adds a listener/handler for a specific event type. 
        /// </summary>
        /// <remarks> 
        /// Note: subscription is always O(n) where n is the number of subscribers to
        /// the given type.
        /// </remarks>
        /// <typeparam name="T"> the type of event to listen for </typeparam>
        /// <param name="callback"> the handler to call when an event of type <typeparamref name="T" /> is published. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null </exception>
        public void Subscribe<T>(Event<T> callback) { Subscribe(typeof(T), Argument.NotNull(callback)); }
        public void Subscribe<T>(AsyncEvent<T> callback) { Subscribe(typeof(T), Argument.NotNull(callback)); }
        internal void Subscribe(Type type, Delegate callback) {
            Assert.IsNotNull(type);
            Assert.IsNotNull(callback);
            List<Delegate> typeSubscribers;
            if (_subscribers.TryGetValue(type, out typeSubscribers)) {
                typeSubscribers.Add(callback);
            } else {
                _subscribers.Add(type, new List<Delegate> { callback });
            }
        }

        /// <summary> 
        /// Removes a listener from a specfic event type. 
        /// </summary>
        /// <remarks> 
        /// Note: unsubscription is always O(n) where n is the number of subscribers to
        /// the given type.
        /// </remarks>
        /// <typeparam name="T"> the type of event to remove the handler from </typeparam>
        /// <param name="callback"> the handler to remove </param>
        /// <exception cref="ArgumentNullException"> <paramref name="callback" /> is null </exception>
        public void Unsubscribe<T>(Event<T> callback) { Unsubscribe(typeof(T), Argument.NotNull(callback)); }
        public void Unsubscribe<T>(AsyncEvent<T> callback) { Unsubscribe(typeof(T), Argument.NotNull(callback)); }
        internal void Unsubscribe(Type type, Delegate callback) {
            List<Delegate> typeSubscribers;
            if (!_subscribers.TryGetValue(type, out typeSubscribers))
                return;
            typeSubscribers.Remove(callback);
            if (typeSubscribers.Count <= 0)
                _subscribers.Remove(type);
        }

        /// <summary> 
        /// Publishes a new event. 
        /// </summary>
        /// <remarks> 
        /// Execution is immediate. All handler code will be executed before returning from this method. 
        /// The event object may be mutated. The object may be altered after execution. 
        /// 
        /// Any asynchronous callbacks will not be awaited. To await the completion of all 
        /// callbacks, use PublishAsync instead.
        /// </remarks>
        /// <param name="evnt"> the event object </param>
        /// <exception cref="ArgumentNullException"> <paramref name="evnt" /> is null </exception>
        public void Publish<T>(T evnt) {
            Type eventType = Argument.NotNull(evnt).GetType();
            log.Debug("Published: " + eventType.Name);
            var handlers = GetEventTypes(eventType)
                    .Where(t => _subscribers.ContainsKey(t))
                    .SelectMany(t => _subscribers[t])
                    .ToArray();
            foreach (var subscriber in handlers) {
                if (subscriber != null)
                    subscriber.DynamicInvoke(evnt);
            }
        }

        /// <summary> 
        /// Publishes a new event. 
        /// </summary>
        /// <remarks> 
        /// Execution is immediate. All handler code will be executed before returning from this method. 
        /// The event object may be mutated. The object may be altered after execution. 
        /// 
        /// All synchronous callbacks will be waited on to complete.
        /// Returned task will resolve only when all event handlers are resulved.
        /// To avoid awaiting asynchronous callbacks, use Publish instead.
        /// </remarks>
        /// <param name="evnt"> the event object </param>
        /// <returns>a ITask that resolves only when all event handlers are finished executing</returns>
        /// <exception cref="ArgumentNullException"> <paramref name="evnt" /> is null </exception>
        public ITask PublishAsync<T>(T evnt) {
            Type eventType = Argument.NotNull(evnt).GetType();
            log.Debug("Published: " + eventType.Name);
            List<ITask> subtasks = null;
            var handlers = GetEventTypes(eventType)
                    .Where(t => _subscribers.ContainsKey(t))
                    .SelectMany(t => _subscribers[t])
                    .ToArray();
            foreach (var subscriber in handlers) {
                if (subscriber == null)
                    continue;
                var result = subscriber.DynamicInvoke(evnt) as ITask;
                if (result != null) {
                    if (subtasks == null)
                        subtasks = new List<ITask>();
                    subtasks.Add(result);
                }
            }
            if (subtasks == null)
                return Task.Resolved;
            return Task.All(subtasks);
        }

        /// <summary> 
        /// Gets the count of subscribers to certain type of event. 
        /// </summary>
        /// <typeparam name="T"> the type of event to check for. </typeparam>
        /// <returns> how many subscribers said event has. </returns>
        public int GetCount<T>() {
            return GetCount(typeof(T)); 
        }

        /// <summary> 
        /// Gets the count of subscribers to certain type of event. 
        /// </summary>
        /// <param name="type"> the type of event to check for. </param>
        /// <returns> how many subscribers said event has </returns>
        public int GetCount(Type type) {
            Argument.NotNull(type);
            List<Delegate> typeSubscribers;
            if (_subscribers.TryGetValue(type, out typeSubscribers))
                return typeSubscribers.Sum(callback => callback.GetInvocationList().Length);
            return 0;
        }

        /// <summary> 
        /// Removes all subscribers from a single event. 
        /// </summary>
        /// <typeparam name="T"> the type of event to remove </typeparam>
        /// <returns> whether it was removed or not </returns>
        public bool Reset<T>() {
            return Reset(typeof(T)); 
        }

        /// <summary> 
        /// Removes all subscribers from a single event.
        /// </summary>
        /// <param name="type"> the type of event to remove </param>
        /// <returns> whether it was removed or not </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null </exception>
        public bool Reset(Type type) {
            return _subscribers.Remove(Argument.NotNull(type)); 
        }

        /// <summary> 
        /// Removes all subscribers from all events. 
        /// </summary>
        /// <returns> whether any events were removed </returns>
        public bool ResetAll() {
            bool success = _subscribers.Count > 0;
            _subscribers.Clear();
            return success;
        }

        Type[] GetEventTypes(Type type) {
            Assert.IsNotNull(type);
            Type[] eventTypes;
            if (_typeCache.TryGetValue(type, out eventTypes))
                return eventTypes;
            Type currentType = type;
            var ancestors = new List<Type>(type.GetInterfaces());
            while (currentType != null) {
                ancestors.Add(currentType);
                currentType = currentType.BaseType;
            }
            eventTypes = ancestors.ToArray();
            _typeCache.Add(type, eventTypes);
            return eventTypes;
        }

    }

}
