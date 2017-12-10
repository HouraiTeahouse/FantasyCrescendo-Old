namespace HouraiTeahouse.SmashBrew {

    public class SmashTimeManager : TimeManager {

        static Player _pausedPlayer;

        // TODO(james7132): Change it so that this is no longer necessary
        public static SmashTimeManager Instance { get; private set; }

        /// <summary> 
        /// Gets or sets the Player that paused the game.  Works only locally. If set to a non-null value, the game will
        /// be paused. If set to a null value, the game will be unpaused. 
        /// </summary>
        public static Player PausedPlayer {
            get { return _pausedPlayer; }
            set {
                _pausedPlayer = value;
                Paused = _pausedPlayer != null;
            }
        }

    }

}