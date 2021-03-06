using HouraiTeahouse.HouraiInput;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    internal static class VectorUtil {

        static int Sign(float x) {
            if (x > 0)
                return 1;
            if (x < 0)
                return -1;
            return 0;
        }

        public static Vector2 Snap(Vector2 newV, Vector2 oldV) {
            if (Sign(newV.x) != Sign(oldV.x))
                oldV.x = 0f;
            if (Sign(newV.y) != Sign(oldV.y))
                oldV.y = 0f;
            return oldV;
        }

    }

    [DisallowMultipleComponent]
    public class InputState : CharacterComponent, IDataComponent<Player> {

        // TODO(james7132): Expose as config option
        const int TapFrameHistory = 5;

        PlayerControlMapping _controlMapping;

        Player Player { get; set; }
        TapDetector _tapDetector;
        Vector2[] _tapHistory;
        int _tapIndex;

        public Vector2 Movement {
            get { 
                Vector2 move = Vector2.zero;
                if (Player != null)
                    move = _controlMapping.Stick(Player.Controller);
                //TODO(james7132): Turn this into an input device
                move.x += ButtonAxis(GetKeys(KeyCode.A, KeyCode.LeftArrow),
                                     GetKeys(KeyCode.D, KeyCode.RightArrow));
                move.y += ButtonAxis(GetKeys(KeyCode.S, KeyCode.DownArrow),
                                     GetKeys(KeyCode.W, KeyCode.UpArrow));
                return DirectionClamp(move);
            }
        }

        public Vector2 Smash {
            get { 
                // TODO(james7132): Do proper smash input detection
                var smash = _tapHistory.Aggregate((lhs, rhs) => new Vector2(AbsMax(lhs.x, rhs.x), AbsMax(lhs.y, rhs.y)));
                smash.x += ButtonAxis(GetKeys(KeyCode.A), GetKeys(KeyCode.D));
                smash.y += ButtonAxis(GetKeys(KeyCode.S), GetKeys(KeyCode.W));
                smash = VectorUtil.Snap(Movement, smash);
                return DirectionClamp(smash);
            }
        }

        public bool Jump {
            get {
                var val = IsValid && _controlMapping.Jump(Player.Controller);
                val |= _controlMapping.TapJump && Smash.y > DirectionalInput.DeadZone;
                return val || GetKeys(KeyCode.W, KeyCode.UpArrow);
            }
        }

        bool IsValid {
            get { return Player != null && Player.Controller != null;}
        }
        float AbsMax(float a, float b) {
            return Mathf.Abs(a) > Mathf.Abs(b) ? a : b;
        }

        public InputSlice GetInput() {
            var valid = IsValid;
            var test = new InputSlice {
                Movement = this.Movement,
                Smash = this.Smash,
                Attack = valid && (GetKeys(KeyCode.E) || _controlMapping.Attack(Player.Controller)),
                Special = valid && (GetKeys(KeyCode.S) || _controlMapping.Special(Player.Controller)),
                Shield = valid && (GetKeys(KeyCode.LeftShift) || _controlMapping.Shield(Player.Controller)),
                Jump = this.Jump
            };
            return test;
        }

        void IDataComponent<Player>.SetData(Player data) {
            Player = data;
        }
        Vector2 DirectionClamp(Vector2 dir) {
            return  new Vector2(Mathf.Clamp(dir.x, -1, 1), Mathf.Clamp(dir.y, -1, 1));
        }
        bool GetKeys(params KeyCode[] keys) {
            return Player != null && Player.ID == 0 && keys.Any(Input.GetKey);
        }
        float ButtonAxis(bool neg, bool pos) {
            return  (neg ? -1 : 0f) + (pos ? 1f : 0f);
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate() {
            if (Player != null)
                _tapHistory[_tapIndex] = _tapDetector.Process(_controlMapping.Stick(Player.Controller), Time.fixedDeltaTime);
            _tapIndex++;
            if (_tapIndex >= _tapHistory.Length)
                _tapIndex = 0;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            //TODO(james7132): Generalize this 
            _controlMapping = new PlayerControlMapping();
            _tapDetector = new TapDetector(DirectionalInput.DeadZone);
            _tapHistory = new Vector2[TapFrameHistory];
            _tapIndex = 0;
        }

    }

    public class TapDetector {

        readonly float _deadZone;
        Vector2 _acceleration;
        Vector2 _value;
        Vector2 _velocity;

        public Vector2 TapVector { get; private set; }

        public TapDetector(float deadZone) { _deadZone = deadZone; }

        Vector2 DeadZone(Vector2 v) {
            if (Mathf.Abs(v.x) < _deadZone)
                v.x = 0f;
            if (Mathf.Abs(v.y) < _deadZone)
                v.y = 0f;
            return v;
        }


        Vector2 MaxComponent(Vector2 src) {
            if (Mathf.Abs(src.y) > Mathf.Abs(src.x))
                src.x = 0f;
            else
                src.y = 0f;
            return src;
        }

        public Vector2 Process(Vector2 input, float deltaT) {
            if (Mathf.Approximately(deltaT, 0f))
                return Vector2.zero;
            Vector2 v = input - _value;
            _acceleration = v - _velocity;
            _velocity = v;
            _value = input;
            TapVector = MaxComponent(VectorUtil.Snap(input, _acceleration));
            return TapVector;
        }

    }

}
