using UnityEngine;
using AbruptLib;
using Asteroids;
using Asteroids.Gravity.Mono;

namespace Sandbox
{
    [RequireComponent(typeof(KeplerBody))]
    public class OrbitalControllerPrototype : MonoBehaviour
    {
        private KeplerBody _body;

        private void Awake()
        {
            _body = GetComponent<KeplerBody>();
        }

        private void Update()
        {
            if (_body == null) return;
            if (Input.GetMouseButton(0))
            {
                var mousePosition3D = TryGetCurrentMouseProjectedPosition();
                if (mousePosition3D != null)
                {
                    var position = transform.position;
                    var direction = mousePosition3D.Value - position;
                    var parent = transform.WrapParent();
                    direction = parent.InverseTransformDirection(direction);
                    var planarDirection = VectorConverter.Convert(direction);
                    var shiftMult = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;
                    _body.LocalVelocity += planarDirection.Normalized * .5 * Time.deltaTime * shiftMult;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_body == null) return;
            if (Input.GetMouseButton(0))
            {
                var mousePosition3D = TryGetCurrentMouseProjectedPosition();
                if (mousePosition3D != null)
                {
                    var parent = transform.WrapParent();
                    var zAxis = parent.PlanarZ;
                    var position = transform.position;
                    var size = GizmosUtil.GetFixedSize(position) * .3f;
                    var direction = mousePosition3D.Value - position;
                    GizmosUtil.DrawPlainArrow(position, direction, zAxis, size);
                }
            }
        }

        private Vector3? TryGetCurrentMouseProjectedPosition()
        {
            var parent = transform.WrapParent();
            var zAxis = parent.PlanarZ;
            var parentPosition = parent.Position;
            var mousePosition = Input.mousePosition;
            var camera = Camera.main;
            var plane = new Plane(zAxis, parentPosition);
            return camera.TryProjectPosition(mousePosition, plane);
        }
    }
}