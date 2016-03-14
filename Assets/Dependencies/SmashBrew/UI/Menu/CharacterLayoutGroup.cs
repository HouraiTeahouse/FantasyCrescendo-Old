using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    ///     A custom layout group created for controlling the layout of the
    ///     individual character select squares on the character select screen
    /// </summary>
    public class CharacterLayoutGroup : LayoutGroup, ILayoutSelfController {
        [SerializeField, Tooltip("The target aspect ratio for the individual children")] private float _childAspectRatio;

        [SerializeField, Tooltip("The target aspect ratio for the overall layout")] private float _selfAspectRatio;
        [SerializeField, Tooltip("The spacing between child elements, in pixels")] private Vector2 _spacing;

        /// <summary>
        ///     <see cref="LayoutGroup.SetLayoutHorizontal" />
        /// </summary>
        public override void SetLayoutHorizontal() {
            SetLayout(true);
        }

        /// <summary>
        ///     <see cref="LayoutGroup.SetLayoutVertical" />
        /// </summary>
        public override void SetLayoutVertical() {
            SetLayout(false);
        }


        /// <summary>
        ///     <see cref="LayoutGroup.CalculateLayoutInputHorizontal" />
        /// </summary>
        public override void CalculateLayoutInputHorizontal() {
            base.CalculateLayoutInputHorizontal();
            SetLayoutInputForAxis(padding.horizontal, padding.horizontal, -1, 0);
        }

        /// <summary>
        ///     <see cref="LayoutGroup.CalculateLayoutInputVertical" />
        /// </summary>
        public override void CalculateLayoutInputVertical() {
            SetLayoutInputForAxis(padding.vertical, padding.vertical, -1, 1);
        }

        /// <summary>
        ///     Builds the full layout of all children along one axis.
        /// </summary>
        /// <param name="axis">which axis to build on, true if vertical, false if horizontal</param>
        private void SetLayout(bool axis) {
            // Givens
            // Child Aspect Ratio
            var availableSpace = rectTransform.rect.size;
            var count = rectChildren.Count;

            if (availableSpace.x / availableSpace.y > _selfAspectRatio) {
                availableSpace.x = availableSpace.y * _selfAspectRatio;
            }

            // Calculated
            var bestRows = 1;
            var bestCols = 1;
            var itemSize = Vector2.zero;
            var isPrime = count <= 3;
            var effectiveCount = count;
            var maxArea = float.MaxValue;

            // Prime numbers tend to generate very poorly made layouts.
            // Repeat until the the layout is generated with a non-prime count
            // Shouldn't need to run more than twice. 
            do {
                for (var rows = 1; rows <= effectiveCount; rows++) {
                    // Reject any "non-rectangle" layouts.
                    if (effectiveCount % rows != 0)
                        continue;
                    isPrime |= rows != 1 && rows != effectiveCount;
                    var cols = effectiveCount / rows;
                    var effectiveSpace = availableSpace -
                                         new Vector2(Mathf.Max(0, cols - 1) * _spacing.x,
                                             Mathf.Max(0, rows - 1) * _spacing.y);
                    var size = new Vector2(effectiveSpace.x / cols, effectiveSpace.y / rows);
                    var area = Mathf.Abs(rows * size.x - cols * size.y * _childAspectRatio);
                    if (area >= maxArea)
                        continue;
                    maxArea = area;
                    bestRows = rows;
                    bestCols = cols;
                    itemSize = size;
                }
                effectiveCount++;
            } while (!isPrime);

            var delta = itemSize + _spacing;

            // Only set the sizes when invoked for horizontal axis, not the positions.

            if (axis) {
                for (var i = 0; i < rectChildren.Count; i++) {
                    var rect = rectChildren[i];

                    m_Tracker.Add(this, rect,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta);

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = itemSize;
                }
                return;
            }

            var center = rectTransform.rect.size / 2;
            var extents = 0.5f * new Vector2(bestCols * itemSize.x + Mathf.Max(0, bestCols - 1) * _spacing.x,
                bestRows * itemSize.y + Mathf.Max(0, bestRows - 1) * _spacing.y);
            var start = center - extents;

            for (var i = 0; i < bestRows; i++) {
                var x = start.x;
                var y = start.y + i * delta.y;
                if (count - i * bestCols < bestCols)
                    x = center.x - 0.5f * (count % bestCols * itemSize.x);

                for (var j = 0; j < bestCols; j++) {
                    var index = bestCols * i + j;
                    if (index >= count)
                        break;

                    SetChildAlongAxis(rectChildren[index], 0, x + j * delta.x, itemSize.x);
                    SetChildAlongAxis(rectChildren[index], 1, y, itemSize.y);
                }
            }
        }
    }
}