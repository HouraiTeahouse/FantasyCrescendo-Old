using System;
using HouraiTeahouse.HouraiInput;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew {

    public class Player {

        PlayerType _type;
        PlayerSelection _selection;
        Character _playerObject;
        NetworkIdentity _networkIdentity;

        internal Player(int number) {
            ID = number;
            Type = PlayerType.Types[0];
            _selection = new PlayerSelection();
            Selection.Changed += () => Changed?.Invoke();
        }

        public int ID { get; private set; }

        public PlayerType Type {
            get { return _type; }
            set {
                bool changed = _type != value;
                _type = Argument.NotNull(value);
                if (changed)
                    Changed?.Invoke();
            }
        }

        public PlayerSelection Selection {
            get { return _selection; }
            set {
                if(_selection.Copy(value))
                    Log.Info($"Set Player {ID}'s selection to {_selection}");
            }
        }

        public Character PlayerObject {
            get { return _playerObject;  }
            set {
                bool changed = _playerObject != value;
                _playerObject = value;
                if (changed) {
                    if (_playerObject != null)
                        _networkIdentity = _playerObject.GetComponent<NetworkIdentity>();
                    Changed?.Invoke();
                }
            }
        }

        public NetworkIdentity NetworkIdentity => _networkIdentity;

        // TODO(james7132): Move this somewhere else
        public InputDevice Controller => Check.Range(ID, HInput.Devices.Count) ? HInput.Devices[ID] : null;

        // The represnetative color of this player. Used in UI.
        public Color Color => Type.Color ?? Config.Player.GetColor(ID);

        public event Action Changed;

        public void CycleType() {
            Type = Type.Next;
            Changed?.Invoke();
        }

        public string GetName(bool shortName = false) {
            return string.Format(shortName ? Type.ShortName : Type.Name, ID + 1);
        }

        public override string ToString() { return GetName(); }

        public override bool Equals(object obj) {
            var player = obj as Player;
            if (player != null)
                return this == player;
            return false;
        }

        public override int GetHashCode() { return ID; }

        public static bool operator ==(Player p1, Player p2) {
            bool n1 = ReferenceEquals(p1, null);
            bool n2 = ReferenceEquals(p2, null);
            return (n1 && n2) || (n1 == n2 && p1.ID == p2.ID);
        }

        public static bool operator !=(Player p1, Player p2) { return !(p1 == p2); }

    }

}