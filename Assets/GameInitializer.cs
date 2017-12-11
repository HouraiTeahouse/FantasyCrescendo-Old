using HouraiTeahouse.SmashBrew.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class GameInitializer : MonoBehaviour {

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            Debug.Log("Initializing game systems..");
            MatchConfigBuilder.Instance.Initialize();
        }

    }

}
