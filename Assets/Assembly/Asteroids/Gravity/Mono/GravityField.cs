using UnityEngine;
using AbruptLib;

namespace Asteroids.Gravity.Mono
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public partial class GravityField : MonoBehaviour, ISerializationCallbackReceiver
    {
        public double Mass { get { return _mass; } }
        public double SurfaceRadius { get { return _surfaceRadius; } }
        public double InfluenceRadius { get { return _influenceRadius; } }
        public MinMax Gravity
        {
            get
            {
                var gMin = _mass / (_influenceRadius * _influenceRadius);
                var gMax = _influenceRadius / _surfaceRadius * gMin;
                return new MinMax(gMin, gMax);
            }
        }

        [SerializeField]
        private double _mass = 10;
        [SerializeField]
        private double _surfaceRadius = 1;
        [SerializeField]
        private double _influenceRadius = 10;
        private bool _initialized;

        public void Modify(double newMass, double newSurfaceRadius, double newInfluenceRadius)
        {
            _mass = newMass;
            _surfaceRadius = newSurfaceRadius;
            _influenceRadius = newInfluenceRadius;
            Validate();
            this.SendMessageToFirstChildren("OnParentGravityFieldModified", SendMessageOptions.DontRequireReceiver);
        }

        public void Modify(double newInfluenceRadius, MinMax newGravity)
        {
            _influenceRadius = newInfluenceRadius;
            _mass = newGravity.Min * _influenceRadius * _influenceRadius;
            if (newGravity.Max == 0) newGravity.Max = 1;
            _surfaceRadius = newGravity.Min / newGravity.Max * _influenceRadius;
            Validate();
            this.SendMessageToFirstChildren("OnParentGravityFieldModified", SendMessageOptions.DontRequireReceiver);
        }

        public Orbit.Motion CreateStableMotion(PlanarVector localPosition)
        {
            var orbit = new Orbit(0, localPosition.Magnitude, 0);
            var angle = orbit.ComputeAngle(localPosition);
            var angleSpeed = orbit.ComputeAngleSpeed(Mass, 1);
            return new Orbit.Motion(orbit, angle, angleSpeed);
        }

        private void Awake()
        {
            Initialize();
        }

        private void Reset()
        {
            Initialize();
        }

        private void OnDrawGizmos()
        {
            var zAxis = transform.TransformDirection(VectorConverter.ConvertedPlanarZ);
            Gizmos.color = new Color(1, 1, 1, .15f);
            GizmosUtil.DrawCircle(transform.position, zAxis, (float)SurfaceRadius, 30);
            GizmosUtil.DrawCircle(transform.position, zAxis, (float)InfluenceRadius, 60);
        }

        private void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            Validate();
            this.SendMessageToFirstChildren("OnParentGravityFieldInitialized", this, SendMessageOptions.DontRequireReceiver);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Validate();
            EditorLazyAction.Add(OnLazyDeserialize);
        }

        private void OnLazyDeserialize()
        {
            if (this == null) return;
            this.SendMessageToFirstChildren("OnParentGravityFieldInitialized", this, SendMessageOptions.DontRequireReceiver);
        }

        // TODO: Sometimes this causes unmanaged behaviour, fix it.
        private void Validate()
        {
            if (_mass < 1e-3) _mass = 1e-3;
            if (_surfaceRadius < 1e-3) _surfaceRadius = 1e-3;
            if (_influenceRadius < _surfaceRadius * 2) _influenceRadius = _surfaceRadius * 2;
        }        
    }
}