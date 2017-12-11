using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary>
    /// A async event handler to load a new scene when a match is resolved.
    /// </summary>
    public class MatchResolver : MonoBehaviour {

        [SerializeField, Scene]
        string _matchCompletionScene;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            var context = Mediator.Global.CreateUnityContext(this);
            context.Subscribe<MatchResolved>(args => {
                Debug.LogFormat("Loading match completion scene ({0})...", _matchCompletionScene);
                return SceneLoader.LoadScene(_matchCompletionScene);
            });
        }

    }

}
