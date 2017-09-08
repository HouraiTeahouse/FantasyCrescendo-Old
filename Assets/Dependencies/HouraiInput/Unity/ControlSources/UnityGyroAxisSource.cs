using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    // This is kind of "beta"... while it works on iOS, gyro controls are
    // inconsistent and are usually fine tuned to the games that use them
    // which is somewhat beyond the scope of this project. But, if you 
    // are curious how to go about it, here you go.
    public class UnityGyroAxisSource : InputSource {

        public enum GyroAxis {
            X = 0,
            Y = 1,
        }

        static Quaternion _zeroAttitude;

        readonly GyroAxis _axis;

        public UnityGyroAxisSource(GyroAxis axis) {
            _axis = axis;
            Calibrate();
        }

        public override float GetValue(InputDevice inputDevice) { 
            return GetAxis()[(int) _axis]; 
        }

        static Quaternion GetAttitude() { 
            return Quaternion.Inverse(_zeroAttitude) * Input.gyro.attitude; 
        }

        static Vector3 GetAxis() {
            Vector3 gv = GetAttitude() * Vector3.forward;
            float gx = ApplyDeadZone(Mathf.Clamp(gv.x, -1.0f, 1.0f));
            float gy = ApplyDeadZone(Mathf.Clamp(gv.y, -1.0f, 1.0f));
            return new Vector3(gx, gy);
        }

        static float ApplyDeadZone(float value) {
            return Mathf.InverseLerp(0.05f, 1.0f, Mathf.Abs(value)) * Mathf.Sign(value);
        }

        public static void Calibrate() { _zeroAttitude = Input.gyro.attitude; }

    }

}