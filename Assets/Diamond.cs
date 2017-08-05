using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    [Serializable]
    public struct Diamond {

        const float yOffset = 0.0005f;

        public Vector2 Center;
        public Vector2 Size;

        public Vector2 Extents {
             get { return Size / 2; }
        }

        public Vector2[] Points {
            get {
                var extents = Extents;
                return new Vector2[] {
                    Center + extents.y * Vector2.up,
                    Center + extents.x * Vector2.right,
                    Center - extents.y * Vector2.up,
                    Center - extents.x * Vector2.right
                };
            }
        }

        public Diamond PhysicsCast(Vector2 direction) {
            var points = Points;
            var baseDistance = direction.magnitude;
            var distance = baseDistance;
            var newCenter = Center + direction;
            for (var i = 0; i < points.Length; i++) {
                var offset = points[i] - Center;
                var internalRaycast = Physics.RaycastAll(Center, offset, offset.magnitude);
                var externalRaycast = Physics.RaycastAll(points[i], direction, baseDistance);
                if (internalRaycast.Length <= 0 && externalRaycast.Length <= 0)
                    continue;
                foreach(var hit in internalRaycast.Concat(externalRaycast)) {
                    if (hit.distance > distance)
                        continue;
                    newCenter = (Vector2)hit.point - offset + (yOffset * Vector2.up);
                    distance = hit.distance;
                    Log.Debug("{0} {1} {2}", Center, offset, newCenter);
                }
            }
            var copy = this;
            copy.Center = newCenter;
            return copy;
        }

        public void Draw(Color? color = null) {
            Color oldColor = Gizmos.color;
            if (color != null)
                Gizmos.color = color.Value;
            var corners = Points;
            for (var i = 0; i < corners.Length; i++)
                Gizmos.DrawLine(corners[i], corners[(i + 1) % corners.Length]);
            if (color != null)
                Gizmos.color = oldColor;
        }

        public void DrawConnects(Diamond target, Color? color = null) {
            Color oldColor = Gizmos.color;
            if (color != null)
                Gizmos.color = color.Value;
            var c1 = Points;
            var c2 = target.Points;
            for (var i = 0; i < c1.Length; i++)
                Gizmos.DrawLine(c1[i], c2[i]);
            if (color != null)
                Gizmos.color = oldColor;
        }
        
    }

}
