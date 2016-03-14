using HouraiTeahouse.HouraiInput;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    public class MenuWiggle : MonoBehaviour {
        [SerializeField] private readonly InputTarget _horizontalAxis = InputTarget.RightStickX;

        [SerializeField] private Vector2 _scale = new Vector2(30, 30);

        [SerializeField] private readonly InputTarget _verticalAxis = InputTarget.RightStickY;

        /// <summary>
        ///     Unity Callback. Called once every frame.
        /// </summary>
        private void Update() {
            var distortion = Vector2.zero;
            foreach (var device in InputManager.Devices) {
                if (device == null)
                    continue;
                float x = device.GetControl(_verticalAxis);
                float y = device.GetControl(_horizontalAxis);
                if (Mathf.Abs(distortion.x) < Mathf.Abs(x))
                    distortion.x = x;
                if (Mathf.Abs(distortion.y) < Mathf.Abs(y))
                    distortion.y = y;
            }
            distortion = Vector2.ClampMagnitude(distortion, 1f);
            distortion.x *= _scale.x;
            distortion.y *= _scale.y;
            transform.rotation = Quaternion.Euler(distortion);
        }
    }
}