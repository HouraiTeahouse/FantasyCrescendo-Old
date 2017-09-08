using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public sealed class AutoDiscover : Attribute {

    }

    public class UnityInputDeviceProfile {

        static readonly HashSet<Type> HiddenTypes = new HashSet<Type>();
        protected string[] JoystickNames;
        protected string[] JoystickRegex;

        protected string LastResortRegex;
        float lowerDeadZone;

        float sensitivity;

        protected string[] SupportedPlatforms;
        float upperDeadZone;

        public UnityInputDeviceProfile() {
            Name = "";
            Meta = "";

            sensitivity = 1.0f;
            lowerDeadZone = 0.2f;
            upperDeadZone = 0.9f;

            AnalogMappings = new InputMapping[0];
            ButtonMappings = new InputMapping[0];
        }

        public string Name { get; protected set; }
        public string Meta { get; protected set; }

        public InputMapping[] AnalogMappings { get; protected set; }
        public InputMapping[] ButtonMappings { get; protected set; }

        public float Sensitivity {
            get { return sensitivity; }
            protected set { sensitivity = Mathf.Clamp01(value); }
        }

        public float LowerDeadZone {
            get { return lowerDeadZone; }
            protected set { lowerDeadZone = Mathf.Clamp01(value); }
        }

        public float UpperDeadZone {
            get { return upperDeadZone; }
            protected set { upperDeadZone = Mathf.Clamp01(value); }
        }

        public bool IsSupportedOnThisPlatform {
            get {
                if (SupportedPlatforms == null || SupportedPlatforms.Length == 0)
                    return true;
                return SupportedPlatforms.Any(platform => HInput.Platform.Contains(platform.ToUpper()));
            }
        }

        public bool IsJoystick {
            get {
                return (LastResortRegex != null) || !JoystickNames.IsNullOrEmpty() || !JoystickRegex.IsNullOrEmpty();
            }
        }

        public bool IsHidden {
            get { return HiddenTypes.Contains(GetType()); }
        }

        public virtual bool IsKnown {
            get { return true; }
        }

        public int AnalogCount {
            get { return AnalogMappings.Length; }
        }

        public int ButtonCount {
            get { return ButtonMappings.Length; }
        }

        public bool HasJoystickName(string joystickName) {
            if (!IsJoystick)
                return false;
            if (JoystickNames != null && JoystickNames.Contains(joystickName, StringComparer.OrdinalIgnoreCase))
                return true;
            return JoystickRegex != null
                && JoystickRegex.Any(t => Regex.IsMatch(joystickName, t, RegexOptions.IgnoreCase));
        }

        public bool HasLastResortRegex(string joystickName) {
            if (!IsJoystick)
                return false;
            return LastResortRegex != null && Regex.IsMatch(joystickName, LastResortRegex, RegexOptions.IgnoreCase);
        }

        public bool HasJoystickOrRegexName(string joystickName) {
            return HasJoystickName(joystickName) || HasLastResortRegex(joystickName);
        }

        public static void Hide(Type type) { 
            HiddenTypes.Add(type); 
        }

        protected static InputSource Button(int index) { 
            return new UnityButtonSource(index); 
        }

        protected static InputSource MouseButton(int index) {
            return new UnityMouseButtonSource(index);
        }

        protected static InputSource Analog(int index) { 
            return new UnityAnalogSource(index); 
        }

        protected static InputSource KeyCodeButton(KeyCode keyCodeList) { 
            return new UnityKeyCodeSource(keyCodeList); 
        }

        protected static InputSource KeyCodeComboButton(params KeyCode[] keyCodeList) {
            return new UnityKeyCodeComboSource(keyCodeList);
        }

        protected static InputSource KeyCodeAxis(KeyCode negativeKeyCode, KeyCode positiveKeyCode) {
            return new UnityKeyCodeAxisSource(negativeKeyCode, positiveKeyCode);
        }

        protected static InputSource MouseXAxis = new UnityMouseAxisSource("x");
        protected static InputSource MouseYAxis = new UnityMouseAxisSource("y");

        protected static InputSource MouseScrollWheel = new UnityMouseAxisSource("z");

    }

}