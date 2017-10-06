using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary>
    /// An async event handler script to play a sequence when a match is completed.
    /// </summary>
    public class MatchCompleteSequence : MonoBehaviour {

        [SerializeField]
        Behaviour _display;

        [SerializeField]
        float _waitForSeconds = 1;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (_display != null)
                _display.enabled =  false;
            var context = Mediator.Global.CreateUnityContext(this);
            context.Subscribe<MatchCompleted>(args => {
                var task = new Task();
                StartCoroutine(RunSequence(task));
                return task;
            });
        }

        IEnumerator RunSequence(ITask task) {
            if (_display != null)
                _display.enabled =  true;
            yield return new WaitForSecondsRealtime(_waitForSeconds);
            task.Resolve();
        }

        /// <summary>
        /// Reset is called when the user hits the Reset button in the Inspector's
        /// context menu or when adding the component the first time.
        /// </summary>
        void Reset() {
            _display = GetComponent<Text>();
        }

    }

}
