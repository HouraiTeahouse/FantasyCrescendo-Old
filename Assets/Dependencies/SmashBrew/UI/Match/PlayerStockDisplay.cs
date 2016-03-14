using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    ///     A behaviour used to display a Player's remaining stock
    /// </summary>
    //TODO: Change to be event based 
    public class PlayerStockDisplay : MonoBehaviour, IDataComponent<Player> {
        private Player _player;

        private StockMatch _stockMatch;

        [SerializeField,
         Tooltip("The Text object used to display the additional stock beyond shown by the simple indicators")] private
            NumberText ExcessDisplay;

        [SerializeField, Tooltip("The standard indicators to show current stock values")] private GameObject[]
            standardIndicators;

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public void SetData(Player data) {
            _player = data;
        }

        /// <summary>
        ///     Unity Callback. Called before the object's first frame.
        /// </summary>
        private void Start() {
            _stockMatch = FindObjectOfType<StockMatch>();
            DisableCheck();
        }

        /// <summary>
        ///     Unity Callback. Called once per frame.
        /// </summary>
        private void Update() {
            DisableCheck();

            if (_stockMatch == null)
                return;

            var stock = _stockMatch[_player];
            var excess = stock > standardIndicators.Length;
            if (ExcessDisplay)
                ExcessDisplay.gameObject.SetActive(excess);
            if (excess) {
                if (ExcessDisplay)
                    ExcessDisplay.Number = stock;
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActive(i == 0);
            }
            else {
                for (var i = 0; i < standardIndicators.Length; i++)
                    if (standardIndicators[i])
                        standardIndicators[i].SetActive(i < stock);
            }
        }

        private void DisableCheck() {
            if (_stockMatch != null && _stockMatch.enabled || _player == null)
                return;
            if (ExcessDisplay)
                ExcessDisplay.gameObject.SetActive(false);
            foreach (var indicator in standardIndicators)
                if (indicator)
                    indicator.SetActive(false);
            enabled = false;
        }
    }
}