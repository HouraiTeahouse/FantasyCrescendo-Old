using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerObjectName : PlayerUIComponent {

        [SerializeField]
        string _format;

        protected override void PlayerChange() { name = string.Format(_format, Player.ID + 1); }

    }

}