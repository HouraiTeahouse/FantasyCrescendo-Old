using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    ///     A SingleActionBehaviour that attaches one object to another.
    /// </summary>
    public class AttachToObject : SingleActionBehaviour {
        [SerializeField, Tooltip("The child to attach to the parent.")] private Transform _child;

        [SerializeField, Tooltip("Whether to keep the child's world position when attaching to the parent")] private
            bool _keepWorldPosition;

        [SerializeField, Tooltip("The parent object to attach the child to")] private Transform _parent;

        [SerializeField, Tooltip("The sibiling index to set the child to. Set as -1 to leave as is.")] private readonly
            int
            _siblingIndex = -1;

        [SerializeField, Tag, Tooltip("The tag to search for if a parent is not provided.")] private string _tag;

        /// <summary>
        ///     Unity callback. Called when the Editor resets the object.
        /// </summary>
        private void Reset() {
            _child = transform;
        }

        /// <summary>
        ///     <see cref="SingleActionBehaviour.Action" />
        /// </summary>
        protected override void Action() {
            var child = _child;
            var parent = _parent;
            if (!parent) {
                var go = GameObject.FindWithTag(_tag);
                if (go)
                    parent = go.transform;
            }
            if (parent) {
                child.SetParent(parent, _keepWorldPosition);
                var rt = parent as RectTransform;
                var crt = child as RectTransform;
                if (_keepWorldPosition && rt && crt) {
                    var canvasRt = rt.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
                }
                if (_siblingIndex >= 0)
                    child.SetSiblingIndex(_siblingIndex);
            }
        }
    }
}