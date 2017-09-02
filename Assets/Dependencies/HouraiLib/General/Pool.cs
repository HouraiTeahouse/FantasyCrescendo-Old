using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace HouraiTeahouse {

    /// <summary> 
    /// An abstract for poolable objects 
    /// </summary>
    /// <typeparam name="T">the type of the object to create</typeparam>
    public abstract class AbstractPool<T> {

        readonly Queue<T> _pool;

        /// <summary> 
        /// Initializes an instance of AbstractPool 
        /// </summary>
        /// <param name="spawnCount"> the number of objects to spawn when the pool is empty </param>
        /// <param name="initialCount"> the number of objects to initially spawn </param>
        protected AbstractPool(int spawnCount, int initialCount = 0) {
            _pool = new Queue<T>();
            SpawnCount = spawnCount;
            Spawn(initialCount);
        }

        /// <summary> 
        /// The number of objects to spawn in when the pool is empty. 
        /// </summary>
        public int SpawnCount { get; set; }

        /// <summary> 
        /// Retrieves an object from the pool. Spawns additional objects if the pool is close to empty. 
        /// </summary>
        /// <returns> the object retrieved or spawned from the pool </returns>
        public virtual T Get() {
            if (_pool.Count <= 0)
                Spawn(SpawnCount);
            T obj = _pool.Dequeue();
            return obj;
        }

        /// <summary> Returns an object to the pool. </summary>
        /// <param name="obj"> the object to return </param>
        /// <exception cref="ArgumentException"> <paramref name="obj" /> is null </exception>
        public virtual void Return(T obj) { 
            _pool.Enqueue(Argument.NotNull(obj)); 
        }

        /// <summary> Spawns a specifed number of instances and adds them to the pool. Does nothing if <paramref name="count" /> is
        /// zero or negative. </summary>
        /// <param name="count"> the number of objects to spawn </param>
        protected void Spawn(int count) {
            for (var i = 0; i < count; i++)
                _pool.Enqueue(Create());
        }

        /// <summary> 
        /// Factory method. Creates a single instance of <typeparamref name="T" /> to add to the pool. 
        /// </summary>
        /// <returns> the new instance </returns>
        protected abstract T Create();

        /// <summary>
        /// Clears all elements currently held in the pool.
        /// Note: Does not release any of the objects created by the pool but not currently managed by it.
        /// </summary>
        public void Clear() {
            _pool.Clear();
        }

    }

    /// <summary> 
    /// A pool of UnityEngine Objects created from a provided prefab. 
    /// </summary>
    /// <typeparam name="T"> the type of object to create and pool </typeparam>
    public class PrefabPool<T> : AbstractPool<T> where T : Object {

        readonly T _source;

        /// <summary> Initializes an instance of PrefabPool </summary>
        /// <param name="source"> the source prefab to copy </param>
        /// <param name="spawnCount"> the number of objects to spawn when the pool is empty </param>
        /// <param name="initialCount"> the number of objects to initially spawn </param>
        /// <exception cref="ArgumentNullException"> <paramref name="source" /> is null </exception>
        public PrefabPool(T source, int spawnCount, int initialCount = 0) : base(spawnCount, 0) {
            _source = Argument.NotNull(source);
            Spawn(initialCount);
        }

        /// <summary> 
        /// Creates an object by instaniating the <see cref="AbstractPool{T}.Create" />
        /// </summary>
        /// <exception cref="InvalidOperationException"> the source prefab has been destroyed </exception>
        protected override T Create() {
            if (!_source)
                throw new InvalidOperationException();
            return Object.Instantiate(_source);
        }

    }

    /// <summary>
    /// A static class for managing simple pools of objects.
    /// Only one pool per type will be created. Generic types with differing generic type parameters
    /// will not share the same pool.
    /// </summary>
    public static class SingletonPool {

        static Dictionary<Type, object> _singletonPools;

        /// <summary>
        /// Retrieves or creates a new pool for a particular type.
        /// </summary>
        /// <returns>the corresponding pool for the type</returns>
        public static AbstractPool<T> Get<T>() where T : new() {
            return Get<T>(() => new T());
        }

        /// <summary>
        /// Retrieves or creates a new pool for a particular type.
        /// </summary>
        /// <param name="createFunc">a parameterless creation function used</param>
        /// <returns>the corresponding pool for the type</returns>
        public static AbstractPool<T> Get<T>(Func<T> createFunc) {
            object pool;
            if (_singletonPools == null)
                _singletonPools = new Dictionary<Type, object>();
            if (!_singletonPools.TryGetValue(typeof(T), out pool)) {
                pool = new EventBasedPool<T>(Argument.NotNull(createFunc), 1);
                _singletonPools.Add(typeof(T), pool);
            }
            return pool as AbstractPool<T>;
        }

    }


    /// <summary> 
    /// A pool that creates objects with a provided creation callback. 
    /// </summary>
    /// <typeparam name="T"> the type of object to create and pool </typeparam>
    public class EventBasedPool<T> : AbstractPool<T> {

        readonly Func<T> _creatFunc;

        /// <summary> Initializes an instance of EventBasedPool. </summary>
        /// <param name="createFunc"> the creation callback to use </param>
        /// <param name="spawnCount"> the number of objects to spawn when the pool is empty </param>
        /// <param name="initialCount"> the number of objects to initially spawn </param>
        /// <exception cref="ArgumentNullException"> <paramref name="createFunc" /> is null. </exception>
        public EventBasedPool(Func<T> createFunc, int spawnCount, int initialCount = 0) : base(spawnCount, 0) {
            _creatFunc = Argument.NotNull(createFunc);
            Spawn(initialCount);
        }

        /// <summary> Creates an object using the provided creation function.
        /// <see cref="AbstractPool{T}.Create" />
        /// </summary>
        protected override T Create() {
            // Should never be null due to check on constructors.
            return _creatFunc();
        }

    }

    /// <summary> A pool that creates new objects with the </summary>
    /// <typeparam name="T"> </typeparam>
    public class Pool<T> : AbstractPool<T> where T : new() {

        /// <summary> Initializes an instance of Pool. </summary>
        /// <param name="spawnCount"> the number of objects to spawn when the pool is empty </param>
        /// <param name="initialCount"> the number of objects to initially spawn </param>
        public Pool(int spawnCount, int initialCount = 0) : base(spawnCount, initialCount) { }

        /// <summary> Creates an object using the parameterless constructor
        /// <see cref="AbstractPool{T}.Create" />
        /// </summary>
        protected override T Create() { return new T(); }

    }

}
