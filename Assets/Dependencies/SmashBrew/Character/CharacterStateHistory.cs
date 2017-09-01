using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew.Characters {

    internal class CharacterStateHistory {

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
            _head = new Record();
            _tail = _head;
        }

        public CharacterStateSummary Advance(InputSlice input, CharacterStateSummary state) {
            Assert.IsNotNull(_tail);
            var newRecord = new Record {
                Next = null,
                Timestamp = _tail.Timestamp + 1,
                Input = input,
            };
            _tail.Next = newRecord;
            Simulate(ref state, _tail, newRecord);
            _tail = newRecord;
            Count++;
            return state;
        }

        public CharacterStateSummary ReconcileState(uint timestamp, CharacterStateSummary state) {
            var newRecord = new Record {
                Timestamp = timestamp,
            };
            if (_head != null) {
                var current = _head;
                Record previous = null;
                var foundSuccessor = false;
                Count = 0;
                while (current != null) {
                    if (current.Timestamp > newRecord.Timestamp) {
                        if (!foundSuccessor) {
                            newRecord.Next = current;
                            previous = newRecord;
                            foundSuccessor = true;
                            Count = 1;
                        }
                        Simulate(ref state, previous, current);
                        Count++;
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