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
        
        MediatorContext _context;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            _context = Mediator.Global.CreateUnityContext(this);
            _context.Subscribe<MatchStarted>(args => {
                foreach (Player player in args.Match.Players) {
                    PlayerIndicator indicator = Instantiate(_prefab);
                    indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                    indicator.gameObject.SetActive(player.Type.IsActive);
                    #if UNITY_EDITOR
                    indicator.gameObject.name = indicator.gameObject.name.Replace("(Clone)", " " + player.ID + 1);
                    #endif
                    if (_container != null)
                        indicator.transform.SetParent(_container.transform, true);
                    _context.Subscribe<PlayerChanged>(playerChangedArgs => {
                        if (playerChangedArgs.Player == player)
                            indicator.gameObject.SetActive(player.Type.IsActive);
                    });
                }
            });
        }

    }

}
