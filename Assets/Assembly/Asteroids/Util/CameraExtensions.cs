using UnityEngine;

namespace Asteroids
{
    public static class CameraExtensions
    {
        public static Vector3? TryProjectPosition(this Camera camera, Vector3 position, Plane plane)
        {
            var ray = camera.ScreenPointToRay(position);
            float distance;
            if (plane.Raycast(ray, out distance))
                return ray.GetPoint(distance);
            return null;
        }
    }
}