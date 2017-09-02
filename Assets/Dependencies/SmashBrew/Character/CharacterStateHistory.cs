using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew.Characters {

    internal class CharacterStateHistory {

        static AbstractPool<Record> _recordPool = SingletonPool.Get<Record>();

        public class Record {
            public Record Next;
            public uint Timestamp;
            public InputSlice Input;
        }

        Record _head;
        Record _tail;

        readonly Character character;
        readonly float deltaTime;

        public int Count { get; private set; }

        public uint LatestTimestamp {
            get { return _tail.Timestamp; }
        }

        public CharacterStateHistory(Character character, float? deltaTime = null) {
            Assert.IsNotNull(character);
            this.character = character;
            this.deltaTime = deltaTime ?? Time.fixedDeltaTime;
            _head = _recordPool.Get();
            _tail = _head;
        }

        public CharacterStateSummary Advance(InputSlice input, CharacterStateSummary state) {
            Assert.IsNotNull(_tail);
            var newRecord = _recordPool.Get();

            newRecord.Next = null;
            newRecord.Timestamp = _tail.Timestamp + 1;
            newRecord.Input = input;

            _tail.Next = newRecord;
            Simulate(ref state, _tail, newRecord);
            _tail = newRecord;
            Count++;
            return state;
        }

        public CharacterStateSummary ReconcileState(uint timestamp, CharacterStateSummary state) {
            var newRecord = _recordPool.Get();

            newRecord.Next = null;
            newRecord.Timestamp = timestamp;
            newRecord.Input = null;

            if (_head != null) {
                var current = _head;
                Record previous = null;
                var foundSuccessor = false;
                Count = 0;
                while (current != null) {
                    if (current.Timestamp > newRecord.Timestamp) {
                        if (!foundSuccessor) {
                            newRecord.Next = previous;
                            newRecord.Input = previous.Input;
                            previous = newRecord;
                            foundSuccessor = true;
                            Count = 1;
                        }
                        Simulate(ref state, previous, current);
                        Count++;
                    } else {
                        _recordPool.Return(current);
                    }
                    previous = current;
                    current = current.Next;
                }
                character.ApplyState(ref state);
            }
            _head = newRecord;
            return state;
        }

        void Simulate(ref CharacterStateSummary state, Record previous, Record current) {
            state = character.Advance(state, deltaTime, new InputContext(previous.Input, current.Input));
        }

    }

}