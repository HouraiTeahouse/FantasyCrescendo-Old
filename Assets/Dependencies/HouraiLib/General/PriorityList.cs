using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse {
    /// <summary>
    ///     An generic iterable priority queue.
    /// </summary>
    /// <typeparam name="T">the type of elements contained by the PriorityList</typeparam>
    public class PriorityList<T> : ICollection<T> {
        // The actual collection. Maps a priority to a bucket of items at that priority.
        // This is a solution to the fact that SortedList does not support duplicate keys
        private readonly SortedList<int, List<T>> _items;

        // The priority cache. In exchange for using a bit more memory. This allows for faster checks
        // like in Contains, which would otherwise involve iterating through every priority cpuLevel and running
        // Contains on every bucket
        private readonly Dictionary<T, int> _priorities;

        /// <summary>
        ///     Creates an empty PriorityList instance
        /// </summary>
        public PriorityList() {
            _items = new SortedList<int, List<T>>();
            _priorities = new Dictionary<T, int>();
        }

        /// <summary>
        ///     Creates a PriorityList instance using the elements from the collection provided.
        ///     All of the elements are assigned to the default priority of 0.
        /// </summary>
        /// <param name="collection"></param>
        /// <exception cref="ArgumentNullException">thrown if collection is null</exception>
        public PriorityList(IEnumerable<T> collection) : this() {
            if (collection == null)
                throw new ArgumentNullException("collection");
            var colList = collection.ToList();
            _items.Add(0, colList);
            foreach (var element in colList)
                _priorities[element] = 0;
        }

        /// <summary>
        ///     Creates a PriorityList instance using the elements and priorities provided by the dictionary.
        ///     Each element is properly mapped to their priority.
        /// </summary>
        /// <param name="priorities"></param>
        public PriorityList(IDictionary<T, int> priorities) {
            if (priorities == null)
                throw new ArgumentNullException("priorities");
            _priorities = new Dictionary<T, int>(priorities);
            foreach (var priority in priorities)
                GetOrCreateBucket(priority.Value).Add(priority.Key);
        }

        /// <summary>
        ///     Gets the priority of an item stored within the PriorityList.
        /// </summary>
        /// <param name="item">the item to get the priority of</param>
        /// <exception cref="ArgumentException">thrown if the PriorityList does not contain this item</exception>
        /// <returns>the priority of the item within the list</returns>
        public int GetPriority(T item) {
            if (!Contains(item))
                throw new ArgumentException("item");
            return _priorities[item];
        }

        /// <summary>
        ///     Gets the priority of an item stored within the PriorityList.
        /// </summary>
        /// <remarks>
        ///     This is for elements already in the PriorityList. To add an element instead, use Add instead.
        /// </remarks>
        /// <exception cref="ArgumentException">thrown if the PriorityList does not contain <paramref name="item" /></exception>
        /// <param name="item">the item to edit the</param>
        /// <param name="priority">the new priority </param>
        public void SetPriority(T item, int priority) {
            if (!Contains(item))
                throw new ArgumentException();

            var current = _priorities[item];

            if (priority == current)
                return;

            var currentBucket = _items[current];
            var newBucket = GetOrCreateBucket(priority);

            // Remove from the old bucket
            currentBucket.Remove(item);
            // Add to the new bucket
            newBucket.Add(item);

            //Remove old bucket if it's empty
            if (currentBucket.Count <= 0)
                _items.Remove(current);

            // Store the new priority
            _priorities[item] = priority;
        }

        /// <summary>
        ///     Removes all elements from the PriorityList with a certain priority
        /// </summary>
        /// <param name="priority">the priority of which to remove all elements of</param>
        /// <returns>whether elements have been removed or not</returns>
        public bool RemoveAllByPriority(int priority) {
            if (!_items.ContainsKey(priority))
                return false;
            var bucket = _items[priority];

            // Remove the bucket
            _items.Remove(priority);

            // Remove all elements in the bucket from the priority cache
            return bucket.Any(element => _priorities.Remove(element));
        }

        /// <summary>
        ///     Removes all elements from the PriorityList between two priorities.
        /// </summary>
        /// <remarks>
        ///     If <paramref name="minPriority" /> is greater than <paramref name="maxPriority" />, the two are swapped.
        ///     If <paramref name="minPriority" /> is equal to <paramref name="maxPriority" />, then this function acts the same
        ///     way RemoveAllByPriority(priority) does.
        /// </remarks>
        /// <param name="minPriority">the minimum priority that should be removed</param>
        /// <param name="maxPriority">the maximum priority that should be removed</param
        /// <returns>whether elements have been removed or not</returns>
        /// >
        public bool RemoveAllByPriority(int minPriority, int maxPriority) {
            if (minPriority > maxPriority) {
                var temp = minPriority;
                minPriority = maxPriority;
                maxPriority = temp;
            }
            if (minPriority == maxPriority)
                return RemoveAllByPriority(minPriority);
            var toRemove = _items.Keys.Where(p => p >= minPriority && p <= maxPriority).ToArray();
            return toRemove.Any(RemoveAllByPriority);
        }

        private List<T> GetOrCreateBucket(int priority) {
            List<T> bucket;
            if (_items.ContainsKey(priority))
                bucket = _items[priority];
            else {
                bucket = new List<T>();
                _items.Add(priority, bucket);
            }
            return bucket;
        }

        #region ICollection Implementation

        public IEnumerator<T> GetEnumerator() {
            return _items.SelectMany(pair => pair.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        ///     Adds a new element to the PriorityList with priority 0.
        /// </summary>
        /// <param name="item">the element to add</param>
        public void Add(T item) {
            Add(item, 0);
        }

        /// <summary>
        ///     Adds a new element to the PriorityList with specified priority.
        /// </summary>
        /// <param name="item">the element to add</param>
        /// <param name="priority"></param>
        public void Add(T item, int priority) {
            var bucket = GetOrCreateBucket(priority);
            bucket.Add(item);
            _priorities[item] = priority;
        }

        public void Clear() {
            _items.Clear();
            _priorities.Clear();
        }

        public bool Contains(T item) {
            return _priorities.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            if (Contains(item))
                return false;
            var priority = _priorities[item];
            var bucket = _items[priority];
            var success = bucket.Remove(item);
            if (success)
                _priorities.Remove(item);
            if (bucket.Count <= 0)
                _items.Remove(priority);
            return true;
        }

        public int Count {
            get { return _priorities.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        #endregion
    }
}