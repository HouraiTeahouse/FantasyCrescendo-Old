using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {

    /// <summary> 
    /// Matches the color between multiple Graphics. 
    /// </summary>
    [ExecuteInEditMode]
    public class MatchColor : MonoBehaviour {

        [SerializeField]
        [Tooltip("The source graphic used to get the base color from.")]
        Graphic _source;

        [SerializeField]
        [Tooltip("The other graphics to synchronize with the source graphic")]
        Graphic[] _targets;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (_source == null)
                _source = GetComponent<Graphic>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            if (_source == null || _targets == null) {
                enabled = false;
                return;
            }

            foreach (Graphic graphic in _targets)
                graphic.color = _source.color;
        }

    }

}