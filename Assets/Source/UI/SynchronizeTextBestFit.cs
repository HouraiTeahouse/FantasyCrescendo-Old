using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// Synchronizes multiple Best-Fit UI Texts to be the same size.
    /// </summary>
    public class SynchronizeTextBestFit : UIBehaviour {

        [SerializeField]
        Text[] _texts;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable() {
            base.OnEnable();
            StartCoroutine(UpdateSizes());
        }

        /// <summary>
        /// This callback is called if an associated RectTransform has its dimensions changed. 
        /// The call is also made to all child rect transforms, even if the child transform itself 
        /// doesn't change - as it could have, depending on its anchoring. 
        /// </summary>
        protected override void OnRectTransformDimensionsChange() {
            foreach (var text in _texts)
                if (text != null)
                    text.resizeTextMaxSize = 10000;
            if (!isActiveAndEnabled)
                return;
            StopAllCoroutines();
            StartCoroutine(UpdateSizes());
        }

        IEnumerator UpdateSizes() {
            // Waits one frame to allow the TextGenerator to properly get a feel for what is the best fit for
            // each text.
            yield return null;
            yield return null;
            while (true) {
                var minSize = _texts.Min(t => t.cachedTextGenerator.fontSizeUsedForBestFit);
                foreach (var text in _texts)
                    text.resizeTextMaxSize = minSize;
                yield return null;
            }
        }

    }
}
