using UnityEngine;

namespace AbruptLib
{
    public static class VectorUtil
    {
        public static Vector3 Clamp(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            return ClampProjection(Project(point, segmentStart, segmentEnd), segmentStart, segmentEnd);
        }

        public static Vector3 Project(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            return segmentStart + Vector3.Project(point - segmentStart, segmentEnd - segmentStart);
        }

        private static Vector3 ClampProjection(Vector3 point, Vector3 start, Vector3 end)
        {
            var toStart = (point - start).sqrMagnitude;
            var toEnd = (point - end).sqrMagnitude;
            var segment = (start - end).sqrMagnitude;
            if (toStart > segment || toEnd > segment) return toStart > toEnd ? end : start;
            return point;
        }

        public static Vector3 Cross(Vector3 normal)
        {
            return Vector3.ProjectOnPlane(new Vector3(normal.y, -normal.x, -normal.z), normal);
        }

        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            return Mathf.Atan2(Vector3.Dot(axis, Vector3.Cross(from, to)), Vector3.Dot(from, to));
        }
    }
}