using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    ///     A CharacterUIComponent that displays the portrait of a character on a RawImage object
    /// </summary>
    public sealed class PortraitDisplay : CharacterUIComponent<RawImage> {
        [SerializeField, Tooltip("Should the character's portrait be cropped?")] private bool _cropped;
        private Rect _cropRect;

        private Color _defaultColor;

        [SerializeField, Tooltip("Tint to cover the potrait, should the character be disabled")] private readonly Color
            _disabledTint = Color.gray;

        [SerializeField, Tooltip("An offset to move the crop rect")] private Vector2 _rectBias;
        private RectTransform _rectTransform;

        /// <summary>
        ///     Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _rectTransform = Component.GetComponent<RectTransform>();
            _defaultColor = Component.color;
        }

        /// <summary>
        ///     <see cref="UIBehaviour.OnRectTransformDimensionsChange" />
        /// </summary>
        protected override void OnRectTransformDimensionsChange() {
            SetRect();
        }

        private void SetRect() {
            if (_rectTransform == null || Component == null || Component.texture == null)
                return;
            var size = _rectTransform.rect.size;
            var aspect = size.x / size.y;
            var texture = Component.texture;
            var imageRect = _cropRect.EnforceAspect(aspect);
            if (imageRect.width > texture.width || imageRect.height > texture.height) {
                imageRect = imageRect.Restrict(texture.width, texture.height, aspect);
                imageRect.center = texture.Center();
            }
            Component.uvRect = texture.PixelToUVRect(imageRect);
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null || data.PalleteCount <= 0)
                return;
            var portrait = Player != null ? Player.Pallete : 0;
            if (data.GetPortrait(portrait).Load() == null)
                return;
            var texture = data.GetPortrait(portrait).Asset.texture;
            _cropRect = _cropped ? data.CropRect(texture) : texture.PixelRect();
            _cropRect.x += _rectBias.x * texture.width;
            _cropRect.y += _rectBias.y * texture.height;
            Component.texture = texture;
            Component.color = data.IsSelectable ? _defaultColor : _disabledTint;
            SetRect();
        }
    }
}