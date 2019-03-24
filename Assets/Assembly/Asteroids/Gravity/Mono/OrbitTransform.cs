using UnityEngine;
using System;

namespace Asteroids.Gravity.Mono
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public partial class OrbitTransform : MonoBehaviour, ISerializationCallbackReceiver
    {
        private const int PathTopology = 250;
        private static readonly PlanarVector[] planarPathBuffer = new PlanarVector[PathTopology];

    #region properties
        public Orbit Orbit
        {
            get { return _orbit; }
            set { SetCurrentOrbit(value, true); }
        }

        public double Angle
        {
            get { return _angle; }
            set { SetCurrentAngle(value, true); }
        }

        public Vector3 Pericenter
        {
            get
            {
                PlanarVector pericenter = _orbit.Pericenter;
                return _transform.WrapParent().TransformPoint(pericenter);
            }
        }

        public Vector3 Apocenter
        {
            get
            {
                PlanarVector apocenter = _orbit.Apocenter;
                return _transform.WrapParent().TransformPoint(apocenter);
            }
        }

        public Path LocalPath
        {
            get
            {
                if (_path == null) _path = new Vector3[PathTopology];
                if (_pathOutdated)
                {
                    _orbit.BuildPath(planarPathBuffer);
                    VectorConverter.Convert(planarPathBuffer, _path);
                    _pathOutdated = false;
                }
                
                return new Path(_path, Orbit.Periodic);
            }
        }
    #endregion properties

        public event Action AngleChanged;

        [SerializeField]
        private Orbit _orbit;
        [SerializeField]
        private double _angle;
        [NonSerialized]
        private Vector3[] _path;
        [SerializeField, HideInInspector]
        private Transform _transform;
        [NonSerialized]
        private bool _pathOutdated = true;
        [NonSerialized]
        private bool _initialized;

        public void Modify(Orbit newOrbit, double newAngle)
        {
            SetCurrentOrbit(newOrbit, false);
            SetCurrentAngle(newAngle, true);
        }

        private void SetCurrentOrbit(Orbit newOrbit, bool updatePosition)
        {
            if (newOrbit == null) throw new ArgumentNullException("Can not set null trajectory.");
            _orbit = newOrbit;
            _pathOutdated = true;
            if (updatePosition) UpdateTransformPositionFromCurrentAngle();
            SendMessage("OnOrbitChanged", newOrbit, SendMessageOptions.DontRequireReceiver);
        }

        private void SetCurrentAngle(double newAngle, bool updatePosition)
        {
            _angle = _orbit.Periodic ? MathUtil.RepeatAngle(newAngle) : newAngle;
            if (updatePosition) UpdateTransformPositionFromCurrentAngle();
            if (AngleChanged != null) AngleChanged.Invoke();
        }

    #region messages
        private void Awake()
        {
            Initialize();
        }

        private void Reset()
        {
            Initialize();
        }

        private void OnTransformParentChanged()
        {
            UpdateTransformPositionFromCurrentAngle();
        }
    #endregion messages

    #region serialization
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _pathOutdated = true;
            EditorLazyAction.Add(OnLazyDeserialize);
        }

        private void OnLazyDeserialize()
        {
            if (this == null) return;
            SendMessage("OnOrbitChanged", Orbit, SendMessageOptions.DontRequireReceiver);
            UpdateTransformPositionFromCurrentAngle();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    #endregion

        private void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            _transform = transform;
            if (_orbit != null) return;
            PlanarVector localPosition = VectorConverter.Convert(_transform.localPosition);
            _orbit = Orbit.CreateSimple(localPosition);
            _angle = _orbit.ComputeAngle(localPosition);
            UpdateTransformPositionFromCurrentAngle();
        }

        private void UpdateTransformPositionFromCurrentAngle()
        {
            PlanarVector newPosition = _orbit.ComputePosition(_angle);
            _transform.localPosition = VectorConverter.Convert(newPosition);
        }
    }
}