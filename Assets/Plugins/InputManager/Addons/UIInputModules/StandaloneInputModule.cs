#region [License]

//	The MIT License (MIT)
//	
//	Copyright (c) 2014, Unity Technologies
//	Copyright (c) 2014, Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//	copies of the Software, and to permit persons to whom the Software is
//	furnished to do so, subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in
//	all copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//	THE SOFTWARE.
//
//	https://bitbucket.org/Unity-Technologies/ui

#endregion

using System;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace TeamUtility.IO {

    [AddComponentMenu("Event/Standalone Input Module")]
    public class StandaloneInputModule : PointerInputModule {

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.",
            false)]
        public enum InputMode {

            Mouse,
            Buttons

        }

        [SerializeField]
        private bool m_AllowActivationOnMobileDevice;

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        private string m_CancelButton = "Cancel";

        [SerializeField]
        private string m_HorizontalAxis = "MenuHorizontal";

        [SerializeField]
        private float m_InputActionsPerSecond = 10;

        private Vector2 m_LastMousePosition;
        private Vector2 m_MousePosition;
        private float m_NextAction;

        /// <summary>
        /// Name of the submit button.
        /// </summary>
        [SerializeField]
        private string m_SubmitButton = "Submit";

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        [SerializeField]
        private string m_VerticalAxis = "MenuVertical";

        protected StandaloneInputModule() {}

        [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.",
            false)]
        public InputMode inputMode {
            get { return InputMode.Mouse; }
        }

        public bool allowActivationOnMobileDevice {
            get { return m_AllowActivationOnMobileDevice; }
            set { m_AllowActivationOnMobileDevice = value; }
        }

        public float inputActionsPerSecond {
            get { return m_InputActionsPerSecond; }
            set { m_InputActionsPerSecond = value; }
        }

        /// <summary>
        /// Name of the horizontal axis for movement (if axis events are used).
        /// </summary>
        public string horizontalAxis {
            get { return m_HorizontalAxis; }
            set { m_HorizontalAxis = value; }
        }

        /// <summary>
        /// Name of the vertical axis for movement (if axis events are used).
        /// </summary>
        public string verticalAxis {
            get { return m_VerticalAxis; }
            set { m_VerticalAxis = value; }
        }

        public string submitButton {
            get { return m_SubmitButton; }
            set { m_SubmitButton = value; }
        }

        public string cancelButton {
            get { return m_CancelButton; }
            set { m_CancelButton = value; }
        }

        public override void UpdateModule() {
            m_LastMousePosition = m_MousePosition;
            m_MousePosition = InputManager.mousePosition;
        }

        public override bool IsModuleSupported() {
            // Check for mouse presence instead of whether touch is supported,
            // as you can connect mouse to a tablet and in that case we'd want
            // to use StandaloneInputModule for non-touch input events.
            return m_AllowActivationOnMobileDevice || InputManager.mousePresent;
        }

        public override bool ShouldActivateModule() {
            if (!base.ShouldActivateModule())
                return false;

            var shouldActivate = InputManager.GetButtonDown(m_SubmitButton);
            shouldActivate |= InputManager.GetButtonDown(m_CancelButton);
            shouldActivate |= !Mathf.Approximately(InputManager.GetAxisRaw(m_HorizontalAxis), 0.0f);
            shouldActivate |= !Mathf.Approximately(InputManager.GetAxisRaw(m_VerticalAxis), 0.0f);
            shouldActivate |= (m_MousePosition - m_LastMousePosition).sqrMagnitude > 0.0f;
            shouldActivate |= InputManager.GetMouseButtonDown(0);
            return shouldActivate;
        }

        public override void ActivateModule() {
            base.ActivateModule();
            m_MousePosition = InputManager.mousePosition;
            m_LastMousePosition = InputManager.mousePosition;

            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.lastSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        public override void DeactivateModule() {
            base.DeactivateModule();
            ClearSelection();
        }

        public override void Process() {
            bool usedEvent = SendUpdateEventToSelectedObject();

            if (eventSystem.sendNavigationEvents) {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }

            ProcessMouseEvent();
        }

        /// <summary>
        /// Process submit keys.
        /// </summary>
        private bool SendSubmitEventToSelectedObject() {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            if (InputManager.GetButtonDown(m_SubmitButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

            if (InputManager.GetButtonDown(m_CancelButton))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
            return data.used;
        }

        private bool AllowMoveEventProcessing(float time) {
            string inputConfigName = InputManager.CurrentConfiguration.name;
            AxisConfiguration hAxisConfig = InputManager.GetAxisConfiguration(inputConfigName, m_HorizontalAxis);
            AxisConfiguration vAxisConfig = InputManager.GetAxisConfiguration(inputConfigName, m_VerticalAxis);

            bool allow = hAxisConfig.AnyKeyDown;
            allow |= vAxisConfig.AnyKeyDown;
            allow |= (time > m_NextAction);
            return allow;
        }

        private Vector2 GetRawMoveVector() {
            Vector2 move = Vector2.zero;
            move.x = InputManager.GetAxisRaw(m_HorizontalAxis);
            move.y = InputManager.GetAxisRaw(m_VerticalAxis);

            if (InputManager.GetButtonDown(m_HorizontalAxis)) {
                if (move.x < 0)
                    move.x = -1f;
                if (move.x > 0)
                    move.x = 1f;
            }
            if (InputManager.GetButtonDown(m_VerticalAxis)) {
                if (move.y < 0)
                    move.y = -1f;
                if (move.y > 0)
                    move.y = 1f;
            }
            return move;
        }

        /// <summary>
        /// Process keyboard events.
        /// </summary>
        private bool SendMoveEventToSelectedObject() {
            float time = Time.unscaledTime;

            if (!AllowMoveEventProcessing(time))
                return false;

            Vector2 movement = GetRawMoveVector();

            // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
            var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
            if (!Mathf.Approximately(axisEventData.moveVector.x, 0f)
                || !Mathf.Approximately(axisEventData.moveVector.y, 0f))
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            m_NextAction = time + 1f/m_InputActionsPerSecond;
            return axisEventData.used;
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        private void ProcessMouseEvent() {
            var mouseData = GetMousePointerEventData();

            var pressed = mouseData.AnyPressesThisFrame();
            var released = mouseData.AnyReleasesThisFrame();

            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            if (!UseMouse(pressed, released, leftButtonData.buttonData))
                return;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
            ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
            ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f)) {
                var scrollHandler =
                    ExecuteEvents.GetEventHandler<IScrollHandler>(
                                                                  leftButtonData.buttonData.pointerCurrentRaycast
                                                                                .gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

        private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData) {
            if (pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling())
                return true;

            return false;
        }

        private bool SendUpdateEventToSelectedObject() {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        private void ProcessMousePress(MouseButtonEventData data) {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame()) {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo,
                                                                pointerEvent,
                                                                ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress) {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                } else
                    pointerEvent.clickCount = 1;

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if (data.ReleasedThisFrame()) {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                else if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if (currentOverGo != pointerEvent.pointerEnter) {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

#if UNITY_EDITOR && (UNITY_4_6 || UNITY_4_7 || UNITY_5)
        [MenuItem("Team Utility/Input Manager/Use Custom Input Module", false, 200)]
        private static void FixEventSystem() {
            UnityEngine.EventSystems.StandaloneInputModule[] im =
                FindObjectsOfType<UnityEngine.EventSystems.StandaloneInputModule>();
            if (im.Length > 0) {
                for (int i = 0; i < im.Length; i++) {
                    im[i].gameObject.AddComponent<StandaloneInputModule>();
                    DestroyImmediate(im[i]);
                }
                EditorUtility.DisplayDialog("Success", "All built-in standalone input modules have been replaced!", "OK");
                Debug.LogFormat("{0} built-in standalone input module(s) have been replaced", im.Length);
            } else
                EditorUtility.DisplayDialog("Warning", "Unable to find any built-in input modules in the scene!", "OK");
        }
#endif
    }

}