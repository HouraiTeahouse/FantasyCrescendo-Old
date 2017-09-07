using HouraiTeahouse.SmashBrew;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

    /// <summary>
    /// A script to select a random character managed by <see cref="DataManager"/> and displays 
    /// their portrait on a provided UI Image.
    /// </summary>
    public class FantasyCrescendoCharacterDisplay : MonoBehaviour {

        [SerializeField]
        Image _image;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Start() {
            var characters = DataManager.Characters;
            if (characters == null)
                return;
            characters.Where(c => c.PalleteCount >= 1)
                .ToArray()
                .Random()
                .GetPortrait(0)
                .LoadAsync()
                .Then(sprite => {
                    if (_image == null)
                        return;
                    if (sprite == null)
                        _image.enabled = false;
                    else
                        _image.sprite = sprite;
                });
        }

    }

}