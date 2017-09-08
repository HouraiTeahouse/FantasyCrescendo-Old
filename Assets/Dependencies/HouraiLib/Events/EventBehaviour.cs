using UnityEngine; 

namespace HouraiTeahouse {

    /// <summary> An abstract class for MonoBehaviours that handle events published by Mediators. </summary>
    /// <typeparam name="T"> the event type to subscribe to </typeparam>
    public abstract class EventBehaviour<T> : MonoBehaviour {

        Mediator _eventManager;

        /// <summary> 
        /// Gets or sets the event manager the event handler is subscribed to. 
        /// </summary>
        public Mediator EventManager {
            get { return _eventManager; }
            set {
                if (_eventManager != null)
                    _eventManager.Unsubscribe<T>(OnEvent);
                _eventManager = value;
                if (_eventManager != null)
                    _eventManager.Subscribe<T>(OnEvent);
            }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake() {
            _eventManager = Mediator.Global;
            _eventManager.Subscribe<T>(OnEvent);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        protected virtual void OnDestroy() {
            if (_eventManager != null)
                _eventManager.Unsubscribe<T>(OnEvent);
        }

        protected abstract void OnEvent(T eventArgs);

    }

}
