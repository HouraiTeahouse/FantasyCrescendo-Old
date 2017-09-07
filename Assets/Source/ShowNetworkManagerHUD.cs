using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo  {

    /// <summary>
    /// Quick and sloppy debug script to show the NetworkManagerHUD when this script is active.
    /// Used as a quick debug method until a proper network lobby UI is created.
    /// </summary>
    public class ShowNetworkManagerHUD : MonoBehaviour {

        NetworkManagerHUD[] huds;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            huds = FindObjectsOfType(typeof(NetworkManagerHUD)).OfType<NetworkManagerHUD>().ToArray();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            State(true);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() {
            State(false);
        }

        void State(bool state) {
            foreach (var hud in huds) {
                if (hud != null)
                    hud.enabled = state;
            }
        }

    }

}