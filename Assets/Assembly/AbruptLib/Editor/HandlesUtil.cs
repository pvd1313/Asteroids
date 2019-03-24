using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AbruptLib.Editor
{
    public static class HandlesUtil
    {
        public static void DrawPolyline(IList<Vector3> points, bool enclosed = false)
        {
            if (points.Count < 2) return;
            for (var i = 0; i < points.Count - 1; i++)
                Handles.DrawLine(points[i], points[i + 1]);
            if (enclosed) Handles.DrawLine(points[0], points[points.Count - 1]);
        }

        public static float DiscRadiusHandle(Quaternion rotation, Vector3 position, float radius)
        {
            Handles.DrawWireDisc(position, rotation * Vector3.up, radius);
            var forward = (rotation * Vector3.forward).normalized;
            var right = (rotation * Vector3.right).normalized;
            var resultRadius = radius;

            resultRadius = ChooseRadius(DiscRadiusDot(position,  forward, radius), radius, resultRadius);
            resultRadius = ChooseRadius(DiscRadiusDot(position, -forward, radius), radius, resultRadius);
            resultRadius = ChooseRadius(DiscRadiusDot(position,  right, radius), radius, resultRadius);
            resultRadius = ChooseRadius(DiscRadiusDot(position, -right, radius), radius, resultRadius);
            return resultRadius;
        }

        private static float ChooseRadius(float newRadius, float realRadius, float currentResultRadius)
        {
            if (newRadius != realRadius) return newRadius;
            return currentResultRadius;
        }

        private static float DiscRadiusDot(Vector3 center, Vector3 direction, float radius)
        {
            var position = center + direction * radius;
            EditorGUI.BeginChangeCheck();
            var newPosition = Handles.Slider(position, direction, HandleUtility.GetHandleSize(position) * .03f, Handles.DotHandleCap, 0);
            if (EditorGUI.EndChangeCheck())
                return Vector3.Distance(center, newPosition);
            return radius;
        }
    }
}