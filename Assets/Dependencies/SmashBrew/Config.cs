using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    [CreateAssetMenu(fileName = "New Config", menuName = "SmashBrew/Config")]
    public sealed class Config : ScriptableObject {
        private static Config _instance;

        public float TangibleSpeedCap {
            get { return _tangibleSpeedCap; }
        }

        public Color CPUColor {
            get { return _cpuColor; }
        }

        public GameMode StandardVersus {
            get { return _standardVersus; }
        }

        public GameMode Training {
            get { return _training; }
        }

        public GameMode Arcade {
            get { return _arcade; }
        }

        public GameMode AllStar {
            get { return _allStar; }
        }

        /// <summary>
        ///     The singleton instance of the game's config
        /// </summary>
        public static Config Instance {
            get {
                if (_instance)
                    return _instance;
                var configs = Resources.LoadAll<Config>(string.Empty);
                if (configs.Length > 0)
                    return _instance = configs[0];
                return _instance = CreateInstance<Config>();
            }
        }

        /// <summary>
        ///     Unity callback. Called on load.
        /// </summary>
        private void OnEnable() {
            GameMode.Current = StandardVersus;
        }

        public Color GetPlayerColor(int playerNumber) {
            return _playerColors[playerNumber % _playerColors.Length];
        }

        public Color GetHitboxColor(Hitbox.Type type) {
            switch (type) {
                case Hitbox.Type.Offensive:
                    return OffensiveHitboxColor;
                case Hitbox.Type.Damageable:
                    return DamageableHitboxColor;
                case Hitbox.Type.Invincible:
                    return IntangibleHitboxColor;
                case Hitbox.Type.Intangible:
                    return InvincibleHitboxColor;
                default:
                    return Color.magenta;
            }
        }

        #region Serialized Fields

        [Header("Players")] [SerializeField] private readonly Color[] _playerColors = {
            Color.red, Color.blue, Color.green, Color.yellow,
            new Color(1, 0.5f, 0), Color.cyan, Color.magenta, new Color(0.25f, 0.25f, 0.25f)
        };

        [SerializeField] private readonly Color _cpuColor = new Color(0.75f, 0.75f, 0.75f);

        [Header("Physics")] [SerializeField] private readonly float _tangibleSpeedCap = 1.5f;

        [Header("Debug")] [SerializeField] private readonly Color DamageableHitboxColor = Color.yellow;

        [SerializeField] private readonly Color IntangibleHitboxColor = Color.blue;

        [SerializeField] private readonly Color InvincibleHitboxColor = Color.green;

        [SerializeField] private readonly Color OffensiveHitboxColor = Color.red;

        [Header("Game Modes")] [SerializeField] private SerializedGameMode _standardVersus;

        [SerializeField] private SerializedGameMode _training;

        [SerializeField] private SerializedGameMode _arcade;

        [SerializeField] private SerializedGameMode _allStar;

        #endregion
    }
}