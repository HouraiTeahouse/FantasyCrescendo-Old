using UnityEngine;

namespace HouraiTeahouse {
    [ExecuteInEditMode]
    public class Dio : MonoBehaviour {
        [SerializeField, Range(0, 1)] private float _innerRatio;

        private Material _mat;

        [SerializeField, Range(0, 1)] private float _outerRatio;

        [SerializeField, HideInInspector] private Shader _shader;
        [SerializeField] private Vector2 center;

        private void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (_mat == null) {
                if (_shader == null) {
                    enabled = false;
                    return;
                }
                _mat = new Material(_shader);
            }

            float aspectRatio = Screen.width / Screen.height;

            _mat.SetVector("_Aspect", new Vector4(aspectRatio, 1, 1, 1));
            _mat.SetVector("_Center", new Vector4(center.x, center.y, _innerRatio, _outerRatio));

            Graphics.Blit(src, dst, _mat);
        }
    }
}