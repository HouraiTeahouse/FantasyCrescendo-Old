using HouraiTeahouse.Events;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    ///     A PrefabFactoryEventHandler that creates
    /// </summary>
    public sealed class PlayerInfoGUI : PrefabFactoryEventHandler<RectTransform, PlayerSpawnEvent> {
        /// <summary>
        ///     The parent RectTransform to attach the spawned objects to.
        /// </summary>
        [SerializeField] private RectTransform _container;

        private RectTransform _finalSpace;

        /// <summary>
        ///     The space prefabs to place before and after all of the elements to keep them centered.
        /// </summary>
        [SerializeField] private RectTransform _spacePrefab;

        /// <summary>
        ///     Unity callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            if (!_container) {
                enabled = false;
                return;
            }
            if (!_spacePrefab)
                return;

            var initialSpace = Instantiate(_spacePrefab);
            _finalSpace = Instantiate(_spacePrefab);

            initialSpace.SetParent(_container.transform);
            _finalSpace.SetParent(_container.transform);

            initialSpace.name = _spacePrefab.name;
            _finalSpace.name = _spacePrefab.name;
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventHandler{T,TEvent}.ShouldCreate" />
        /// </summary>
        protected override bool ShouldCreate(PlayerSpawnEvent eventArgs) {
            return base.ShouldCreate(eventArgs) && eventArgs.Player != null;
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventHandler{T,TEvent}.Create" />
        /// </summary>
        protected override RectTransform Create(PlayerSpawnEvent eventArgs) {
            var player = eventArgs.Player;
            var display = base.Create(eventArgs);
            display.transform.SetParent(_container.transform, false);
            LayoutRebuilder.MarkLayoutForRebuild(display);
            display.GetComponentsInChildren<IDataComponent<Player>>().SetData(player);
            _finalSpace.transform.SetAsLastSibling();
            return display;
        }
    }
}