using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> 
    /// A set of useful utility funcitons for the UnityEngine Rect struct 
    /// </summary>
    public static class RectExtensions {

        /// <summary> 
        /// Changes the width of a Rect without changing the centered position of the Rect. 
        /// </summary>
        /// <param name="rect"> the source Rect </param>
        /// <param name="width"> the new width of the Rect </param>
        /// <returns> the edited Rect </returns>
        public static Rect ChangeWidth(this Rect rect, float width) {
            float diff = rect.width - width;
            rect.width = width;
            rect.x += diff / 2;
            return rect;
        }

        /// <summary> 
        /// Changes the height of a Rect without changing the centered posiiton of the Rect. 
        /// </summary>
        /// <param name="rect"> the source Rect </param>
        /// <param name="height"> the new height of the Rect </param>
        /// <returns> the edited Rect </returns>
        public static Rect ChangeHeight(this Rect rect, float height) {
            float diff = rect.height - height;
            rect.height = height;
            rect.y += diff / 2;
            return rect;
        }

        /// <summary> 
        /// Changes both the height and width of the a Rect without changing the centered position of the Rect. 
        /// </summary>
        /// <param name="rect"> the source Rect </param>
        /// <param name="width"> the new width of the Rect </param>
        /// <param name="height"> the new height of the Rect </param>
        /// <returns> the edited Rect </returns>
        public static Rect ChangeDims(this Rect rect, float width, float height) {
            return rect.ChangeWidth(width).ChangeHeight(height);
        }

        /// <summary> 
        /// Changes both the height and width of the a Rect without changing the centered position of the Rect. 
        /// </summary>
        /// <param name="rect"> the source Rect </param>
        /// <param name="dims"> the new dimensions of the Rect </param>
        /// <returns> the edited Rect </returns>
        public static Rect ChangeDims(this Rect rect, Vector2 dims) {
            return rect.ChangeDims(dims.x, dims.y);
        }

        /// <summary> 
        /// Gets the aspect ratio of the rect. If the height of the Rect is zero, returns NaN. 
        /// </summary>
        /// <param name="rect"> the Rect to get the aspect ratio of </param>
        /// <returns> the aspect ratio of the Rect </returns>
        public static float AspectRatio(this Rect rect) {
            if (Mathf.Approximately(rect.height, 0f))
                return float.NaN;
            return rect.width / rect.height;
        }

        /// <summary> 
        /// Enforces an aspect ratio on a Rect. Will expand the Rect if necessary 
        /// </summary>
        /// <param name="rect"> the source Rect </param>
        /// <param name="aspect"> the aspect ratio to enforce </param>
        /// <returns> the edited Rect </returns>
        public static Rect EnforceAspect(this Rect rect, float aspect) {
            if (rect.AspectRatio() < aspect) {
                // Image is wider than cropRect, extend it sideways
                return rect.ChangeWidth(aspect * rect.height);
            }
            // Image is wider than cropRect, extend it vertically
            return rect.ChangeHeight(rect.width / aspect);
        }

        /// <summary> 
        /// Restricts a Rect to to a certain size. Retains the center. Optionally can enforce an aspect ratio. 
        /// </summary>
        /// <param name="rect"> the source Rect </param>
        /// <param name="size"> the maximum dimensions of the new Rect </param>
        /// <param name="aspect"> the aspect ratio to enforce, will not enforce one if null </param>
        /// <returns> the edited Rect </returns>
        public static Rect Restrict(this Rect rect, Vector2 size, float? aspect = null) {
            return rect.Restrict(size.x, size.y, aspect);
        }

        /// <summary> 
        /// Restricts a Rect to to a certain size. Retains the center. Optionally can enforce an aspect ratio. 
        /// </summary>
        /// <param name="rect"> the source Rect </param>
        /// <param name="width"> the maximum width of the new Rect </param>
        /// <param name="height"> the maximum height of the new Rect </param>
        /// <param name="aspect"> the aspect ratio to enforce, will not enforce one if null </param>
        /// <returns> the edited Rect </returns>
        /// <exception cref="ArgumentException"> <paramref name="height" /> is zero </exception>
        public static Rect Restrict(this Rect rect, float width, float height, float? aspect = null) {
            Vector2 center = rect.center;
            if (aspect == null) {
                rect.width = Mathf.Min(rect.width, width);
                rect.height = Mathf.Min(rect.height, height);
            }
            else {
                if (Mathf.Approximately(height, 0))
                    throw new ArgumentException();
                float enclosingAspectRatio = width / height;
                float aspectRatio = aspect.Value;
                if (aspectRatio < enclosingAspectRatio) {
                    rect.width = width;
                    rect.height = width / aspectRatio;
                }
                else {
                    rect.height = height;
                    rect.width = height * aspectRatio;
                }
            }
            rect.center = center;
            return rect;
        }

    }

}