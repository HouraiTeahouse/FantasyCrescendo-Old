using HouraiTeahouse.HouraiInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {
    public class SmashBrewInputModule : PointerInputModule {
        [SerializeField] private readonly InputTarget _cancelGamepad = InputTarget.Action2;

        [SerializeField] private readonly string _cancelKeyboard = "cancel";

        [SerializeField] private readonly float _deadZone = 0.1f;

        [SerializeField] private readonly InputTarget _horizontalGamepad = InputTarget.LeftStickX;
        [SerializeField] private readonly string _horizontalKeyboard = "horizontal";

        [SerializeField] private readonly float _navigationDelay = 0.25f;

        [SerializeField] private readonly InputTarget _submitGamepad = InputTarget.Action1;

        [SerializeField] private readonly string _submitKeyboard = "submit";

        [SerializeField] private readonly InputTarget _verticalGamepad = InputTarget.LeftStickY;

        [SerializeField] private readonly string _verticalKeyboard = "vertical";

        private float currentDelay;

        /// <summary>
        ///     Called when the InputModule is activated.
        /// </summary>
        public override void ActivateModule() {
            base.ActivateModule();

            if (!eventSystem.currentSelectedGameObject)
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject, GetBaseEventData());
        }

        /// <summary>
        ///     Called when the InputModule is deactivated.
        /// </summary>
        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

        /// <summary>
        ///     Called once every frame while InputModule is active.
        /// </summary>
        public override void Process() {
            if (!eventSystem)
                return;
            var target = eventSystem.currentSelectedGameObject;

            var submit = Input.GetButtonDown(_submitKeyboard);
            var cancel = Input.GetButtonDown(_cancelKeyboard);

            var x = Input.GetAxisRaw(_horizontalKeyboard);
            var y = Input.GetAxisRaw(_verticalKeyboard);
            var count = 1;
            foreach (InputDevice device in HInput.Devices) {
                if (device == null)
                    continue;
                x += device.GetControl(_horizontalGamepad);
                y += device.GetControl(_verticalGamepad);
                submit |= device.GetControl(_submitGamepad).WasPressed;
                cancel |= device.GetControl(_cancelGamepad).WasPressed;
                count++;
            }

            if (submit)
                ExecuteEvents.Execute(target, GetBaseEventData(), ExecuteEvents.submitHandler);
            if (cancel)
                ExecuteEvents.Execute(target, GetBaseEventData(), ExecuteEvents.cancelHandler);

            currentDelay -= Time.deltaTime;
            if (!eventSystem.sendNavigationEvents || currentDelay >= 0)
                return;

            x /= count;
            y /= count;
            var moveData = GetAxisEventData(x, y, _deadZone);
            ExecuteEvents.Execute(target, moveData, ExecuteEvents.moveHandler);
            if (moveData.moveDir != MoveDirection.None)
                currentDelay = _navigationDelay;
        }
    }
}
