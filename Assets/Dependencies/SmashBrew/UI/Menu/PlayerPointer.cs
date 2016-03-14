using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {
    public class PlayerPointer : PlayerUIComponent {
        private CharacterSelectInputModule _inputModule;

        [SerializeField] private readonly float _movementSpeed = 20;
        private RectTransform _rectTransform;

        protected override void Awake() {
            base.Awake();
            _rectTransform = transform as RectTransform;
            var inputModule = CharacterSelectInputModule.Instance;
            if (!inputModule) {
                Destroy(this);
                return;
            }
            inputModule.AddPointer(this);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_inputModule)
                _inputModule.RemovePointer(this);
        }

        public void Move(Vector2 movement) {
            if (!Player.IsActive && movement != Vector2.zero)
                Player.CycleType();
            var bounds = new Bounds(Vector3.zero,
                (_rectTransform.parent as RectTransform).rect.size - 0.5f * _rectTransform.sizeDelta);
            _rectTransform.anchoredPosition =
                bounds.ClosestPoint(_rectTransform.anchoredPosition + _movementSpeed * movement * Time.deltaTime);
        }
    }
}