using UnityEngine;

namespace UnityStandardAssets.ImageEffects {
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
    public class Grayscale : ImageEffectBase {
        [Range(-1.0f, 1.0f)] public float rampOffset;
        public Texture textureRamp;

        // Called by camera to apply image effect
        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            material.SetTexture("_RampTex", textureRamp);
            material.SetFloat("_RampOffset", rampOffset);
            Graphics.Blit(source, destination, material);
        }
    }
}