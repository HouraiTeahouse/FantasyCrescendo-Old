using HouraiTeahouse.SmashBrew.Matches;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A PrefabFactoryEventHandler that creates </summary>
    public sealed class PlayerInfoGUI : MonoBehaviour {

        [SerializeField]
        RectTransform _prefab;

        /// <summary> The parent RectTransform to attach the spawned objects to. </summary>
        [SerializeField]
        RectTransform _container;

        RectTransform _finalSpace;

        /// <summary> The space prefabs to place before and after all of the elements to keep them centered. </summary>
        [SerializeField]
        RectTransform _spacePrefab;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            if (!_container) {
                enabled = false;
                return;
            }
            if (!_spacePrefab)
                return;

            RectTransform initialSpace = Instantiate(_spacePrefab);
            _finalSpace = Instantiate(_spacePrefab);

            initialSpace.SetParent(_container.transform);
            _finalSpace.SetParent(_container.transform);

            // initialSpace.name = _spacePrefab.name;
            // Debug.LogError("{0} {1}", _finalSpace, _spacePrefab);
            // _finalSpace.name = _spacePrefab.name;

            var context = Mediator.Global.CreateUnityContext(this);
            context.Subscribe<PlayerSpawnEvent>(playerSpawnArgs => {
                var player = playerSpawnArgs.Player;
                RectTransform display = Instantiate(_prefab);
                display.transform.SetParent(_container.transform, false);
                LayoutRebuilder.MarkLayoutForRebuild(display);
                display.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
                display.name = string.Format("Player {0} Display", player.ID + 1);
                display.gameObject.SetActive(player.Type.IsActive);
                context.Subscribe<PlayerChanged>(args => {
                    if (args.Player == player)
                        display.gameObject.SetActive(player.Type.IsActive);
                });
                _finalSpace.transform.SetAsLastSibling();
            });
        }

    }

}
