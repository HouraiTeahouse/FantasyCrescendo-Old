using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> 
    /// Static class for editing the global time properties of the game. Allows for pausing of the game and altering
    /// the global time scale. Inherits from MonoBehaviour. A custom Editor allows editing the pause/timescale state of the
    /// game from the Editor. 
    /// </summary>
    public class TimeManager : Singleton<TimeManager> {

        static float _timeScale = 1f;
        static bool _paused;

        /// <summary> 
        /// Gets or sets whether the game is paused or not. Changing this value will fire the OnPause event If the value
        /// is the same, nothing will change. If the game is paused, Time.timeScale will be set to 0. When unpaused, Time.timeScale
        /// will be set to the value of TimeScale 
        /// </summary>
        public static bool Paused {
            get { return _paused; }
            set {
                if (_paused == value)
                    return;
                _paused = value;
                Time.timeScale = _paused ? 0f : TimeScale;
                Mediator.Global.Publish(new PausedStateChange {
                    IsPaused = value
                });
            }
        }

        /// <summary> 
        /// Gets or sets the global timescale of the game. 
        /// If the game is not paused, Time.timeScale will also be set to
        /// the same value 
        /// </summary>
        public static float TimeScale {
            get { return _timeScale; }
            set {
                if (Mathf.Approximately(_timeScale, value))
                    return;
                _timeScale = value;
                if (!Paused)
                    Time.timeScale = value;
                Mediator.Global.Publish(new TimeScaleChange{
                    TimeScale = value
                });
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _timeScale = Time.timeScale;
        }

    }

    public class PausedStateChange {
        public bool IsPaused { get; set; }
    }

    public struct TimeScaleChange {
        public float TimeScale;
    }

}