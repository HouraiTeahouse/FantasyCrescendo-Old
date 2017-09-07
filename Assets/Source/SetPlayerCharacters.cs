using HouraiTeahouse.SmashBrew;
using UnityEngine;
using UnityEngine.EventSystems;

// DEPRECATED: No longer used.
// TODO(james7132): Remove from the code base and all references to it.
public class SetPlayerCharacters : MonoBehaviour, ISubmitHandler {

    [SerializeField]
    CharacterData character;

    public void OnSubmit(BaseEventData eventData) {
        //foreach (Player player in ) {
        //    player.Selection = new PlayerSelection {Character = character, Pallete = 0};
        //    player.Type = player.ID < 2 ? PlayerType.HumanPlayer : PlayerType.None;
        //}
    }

}
