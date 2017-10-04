using HouraiTeahouse.SmashBrew.Matches;
using System;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> 
    /// A PrefabFactoryEventHandler that produces PlayerIndicators in response to Players spawning. 
    /// </summary>
    public sealed class PlayerIndicatorFactory : MonoBehaviour {

        [SerializeField]
        PlayerIndicator _prefab;

        [SerializeField]
        Canvas _container;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            foreach (Player player in Match.Current.Players) {
                PlayerIndicator indicator = Instantiate(_prefab);
                indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                indicator.gameObject.SetActive(player.Type.IsActive);
                #if UNITY_EDITOR
                indicator.gameObject.name = indicator.gameObject.name.Replace("(Clone)", $" {player.ID + 1}");
                #endif
                if (_container != null)
                    indicator.transform.SetParent(_container.transform, true);
                player.Changed += () => indicator.gameObject.SetActive(player.Type.IsActive);
            }
        }

    }

}
