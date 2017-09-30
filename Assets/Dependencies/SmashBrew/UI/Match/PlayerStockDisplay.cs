using HouraiTeahouse.SmashBrew.Matches;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A behaviour used to display a Player's remaining stock </summary>
    //TODO: Change to be event based 
    public class PlayerStockDisplay : PlayerUIComponent {

        [SerializeField]
        [Tooltip("The Text object used to display the additional stock beyond shown by the simple indicators")]
        NumberText ExcessDisplay;

        [SerializeField]
        [Tooltip("The standard indicators to show current stock values")]
        GameObject[] standardIndicators;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            var character = Player?.PlayerObject;
            if (character == null) {
                ExcessDisplay.gameObject.SetActive(false);
                foreach (GameObject standardIndicator in standardIndicators)
                    standardIndicator.SetActive(false);
                return;
            }

            var stock = character.State.Stocks;
            bool excess = stock > standardIndicators.Length;
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

    }

}
