using UnityEngine;
using System.Collections.Generic;

namespace AbruptLib
{
    public static class GizmosUtil
    {
        public static void DrawPlainArrow(Vector3 origin, Vector3 direction, Vector3 normal, float size)
        {
            var camera = Camera.current;
            var head = origin + direction.normalized * size;
            var sideDirection = (origin - head) / 2;
            var right = head + Quaternion.AngleAxis( 30, normal) * sideDirection;
            var left  = head + Quaternion.AngleAxis(-30, normal) * sideDirection;
            Gizmos.DrawLine(head, right);
            Gizmos.DrawLine(head, left);
            Gizmos.DrawLine(left, right);
        }

        public static void DrawCircle(Vector3 center, Vector3 plainNormal, float radius, int topology)
        {
            if (topology < 3) return;
            var direction = Vector3.ProjectOnPlane(plainNormal + Vector3.one, plainNormal).normalized * radius;
            var positions = new Vector3[topology];
            var deltaRotate = 360f / topology;
            for (var i = 0; i < topology; i++) positions[i] = center + Quaternion.AngleAxis(i * deltaRotate, plainNormal) * direction;
            DrawPolyline(positions, true);
        }

        public static void DrawPolyline(IList<Vector3> points, bool enclosed = false)
        {
            if (points.Count < 2) return;
            for (var i = 0; i < points.Count - 1; i++)
                Gizmos.DrawLine(points[i], points[i + 1]);
            if (enclosed) Gizmos.DrawLine(points[0], points[points.Count - 1]);
        }

        public static float GetFixedSize(Vector3 position)
        {
            var camera = Camera.main;
            var distance = Vector3.Distance(camera.transform.position, position);
            return distance * camera.fieldOfView * .0025f;
        }

        /*public static void DrawPlainAxis(Vector3 axisStart, Vector3 axisEnd, Vector3 up)
        {
            var direction = axisEnd - axisStart;
            Gizmos.DrawLine(axisStart, axisEnd);
            Gizmos.DrawLine(axisStart, axisStart - direction);
            direction = Vector3.Cross(direction, up);
            Gizmos.DrawLine(axisStart, axisStart + direction);
            Gizmos.DrawLine(axisStart, axisStart - direction);
        }*/
    }
}
