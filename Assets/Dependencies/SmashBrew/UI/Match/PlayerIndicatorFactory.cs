using HouraiTeahouse.Events;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    ///     A PrefabFactoryEventHandler that produces PlayerIndicators in response to Players spawning.
    /// </summary>
    public sealed class PlayerIndicatorFactory : PrefabFactoryEventHandler<PlayerIndicator, PlayerSpawnEvent> {
        /// <summary>
        ///     <see cref="AbstractFactoryEventHandler{T,TEvent}.Create" />
        /// </summary>
        protected override PlayerIndicator Create(PlayerSpawnEvent eventArgs) {
            var indicator = base.Create(eventArgs);
            indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(eventArgs.Player);
            return indicator;
        }
    }
}