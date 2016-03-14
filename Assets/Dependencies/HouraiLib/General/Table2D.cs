using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse {
    /// <summary>
    ///     A 2D HashTable. One that requires two keys to access the value.
    /// </summary>
    /// <typeparam name="K1">the type of the first key</typeparam>
    /// <typeparam name="K2">the type of the second key</typeparam>
    /// <typeparam name="V">the type of the values stored</typeparam>
    public class Table2D<K1, K2, V> : Dictionary<K1, Dictionary<K2, V>> {
        /// <summary>
        ///     Gets or sets the value associated with the specified pair of keys.
        /// </summary>
        /// <param name="key1">the first key</param>
        /// <param name="key2">the second key</param>
        /// <returns>the value associated with the pair of keys</returns>
        public virtual V this[K1 key1, K2 key2] {
            get { return this[key1][key2]; }
            set {
                if (!ContainsKey(key1))
                    this[key1] = new Dictionary<K2, V>();
                this[key1][key2] = value;
            }
        }

        /// <summary>
        ///     Adds a key, key, value triplet to the table.
        /// </summary>
        /// <exception cref="ArgumentException">thrown if the Table2D already contains an record with those two keys</exception>
        /// <param name="key1">the first key</param>
        /// <param name="key2">the first key</param>
        /// <param name="value">the associated value</param>
        public virtual void Add(K1 key1, K2 key2, V value) {
            if (ContainsKey(key1, key2))
                throw new ArgumentException();
            this[key1, key2] = value;
        }

        /// <summary>
        ///     Checks if the table contains a value mapped to a set of two keys
        /// </summary>
        /// <param name="key1">the first key</param>
        /// <param name="key2">the seocnd key</param>
        /// <returns>true if the table contains a triplet with the keys, false otherwise</returns>
        public virtual bool ContainsKey(K1 key1, K2 key2) {
            return ContainsKey(key1) && this[key1].ContainsKey(key2);
        }

        /// <summary>
        ///     Removes a key, key, value triplet from the table.
        /// </summary>
        /// <param name="key1">the first key</param>
        /// <param name="key2">the second key</param>
        /// <returns>whether a value was removed or not</returns>
        public virtual bool Remove(K1 key1, K2 key2) {
            if (!ContainsKey(key1))
                return false;
            var row = this[key1];
            var success = row.Remove(key2);
            if (row.Count <= 0)
                Remove(key1);
            return success;
        }

        /// <summary>
        ///     Removes all elements with a second key of a certain value.
        /// </summary>
        /// <param name="key2">the second key</param>
        /// <returns>the count of elements removed</returns>
        public int Remove(K2 key2) {
            return Values.Count(row => row.Remove(key2));
        }
    }

    /// <summary>
    ///     A Table2D with identical first and second key types.
    /// </summary>
    /// <typeparam name="K">the type of the keys</typeparam>
    /// <typeparam name="V">the value stored by the table</typeparam>
    public class Table2D<K, V> : Table2D<K, K, V> {
    }

    /// <summary>
    ///     A mirrored Table2D. The keysets are mirroed.
    ///     If the keyset (a, b) exists, then the keyset (b, a) also exists, and they both map to the
    ///     same value.
    /// </summary>
    /// <typeparam name="K">the type of the keys</typeparam>
    /// <typeparam name="V">the value stored by the table</typeparam>
    public class MirroredTable2D<K, V> : Table2D<K, V> {
        /// <summary>
        ///     Gets or sets the value associated with the specified pair of keys.
        ///     If (a, b) does not exist, then (b, a) is gotten/set.
        /// </summary>
        /// <exception cref="KeyNotFoundException">thrown if both (key1, key2) and (key2, key1) do not exist when using the getter.</exception>
        /// <param name="key1">one of the keys</param>
        /// <param name="key2">one of the keys</param>
        /// <returns>the value stored at (key1, key2)</returns>
        public override V this[K key1, K key2] {
            get {
                if (base.ContainsKey(key1, key2))
                    return base[key1, key2];
                if (base.ContainsKey(key2, key1))
                    return base[key2, key2];
                throw new KeyNotFoundException();
            }
            set {
                if (base.ContainsKey(key1, key2))
                    base[key1, key2] = value;
                else
                    base[key2, key1] = value;
            }
        }

        /// <summary>
        ///     Removes a key, key, value triplet from the table.
        /// </summary>
        /// <param name="key1">one of the keys</param>
        /// <param name="key2">one of the keys</param>
        /// <returns>whether a value was removed or not</returns>
        public override bool Remove(K key1, K key2) {
            // if the first one removes, the or short-circuits and the second is not called
            // this is OK as only one of them should contain the value, if at all.
            return base.Remove(key1, key2) || base.Remove(key2, key1);
        }

        /// <summary>
        ///     Checks if the table contains a value mapped to a set of two keys
        /// </summary>
        /// <param name="key1">the first key</param>
        /// <param name="key2">the seocnd key</param>
        /// <returns>true if the table contains a triplet with the keys, false otherwise</returns>
        /// >
        public override bool ContainsKey(K key1, K key2) {
            return base.ContainsKey(key1, key2) || base.ContainsKey(key2, key1);
        }
    }
}