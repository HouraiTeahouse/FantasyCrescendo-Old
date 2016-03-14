using System;
using System.Collections.Generic;

namespace HouraiTeahouse.SmashBrew {
    public class Counter {
        public float Count { get; private set; }

        public void Increment(float value = 1f) {
            Count += value;
        }

        public static Counter operator ++(Counter counter) {
            if (counter == null)
                throw new NullReferenceException();
            counter.Increment();
            return counter;
        }

        public static Counter operator +(Counter counter, float value) {
            if (counter == null)
                throw new NullReferenceException();
            counter.Increment(value);
            counter += 10;
            return counter;
        }
    }

    public sealed class StatLogger {
        private readonly Dictionary<string, Counter> _counters;

        public StatLogger() {
            _counters = new Dictionary<string, Counter>();
        }

        public Counter this[string counterName] {
            get { return GetCounter(counterName); }
        }

        public Counter GetCounter(string counterName) {
            Counter counter;
            if (_counters.ContainsKey(counterName))
                counter = _counters[counterName];
            else {
                counter = new Counter();
                _counters[counterName] = counter;
            }
            return counter;
        }
    }
}