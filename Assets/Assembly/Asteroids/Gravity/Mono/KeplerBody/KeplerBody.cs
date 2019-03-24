using UnityEngine;
using AbruptLib;
using System.Collections.Generic;
using System;

namespace Asteroids.Gravity.Mono
{
    [ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(OrbitTransform))]
    public partial class KeplerBody : MonoBehaviour
    {
        private const int PATH_TOPOLOGY = 50;
        //private static readonly PlanarVector[] PLANAR_PATH_BUFFER = new PlanarVector[PATH_TOPOLOGY];

        private static bool ParentChangedManually;
        private static bool OrbitChangedManually;
        private static bool AngleChangedManually;
        private static List<Vector3> PointsBuffer = new List<Vector3>();

        public GravityField Parent { get { return _parent; } }

        public GravityField GrandParent
        {
            get
            {
                if (_parent == null) return null;
                var parentBody = _parent.GetComponent<KeplerBody>();
                if (parentBody == null) return null;
                return parentBody._parent;
            }
        }

        public Orbit.Motion Motion
        {
            get { return new Orbit.Motion(_orbitTransform.Orbit, _orbitTransform.Angle, _angleSpeed); }
            set
            {
                OrbitChangedManually = true;
                AngleChangedManually = true;
                _orbitTransform.Modify(value.Orbit, value.Angle);
                _angleSpeed = value.AngleSpeed;
                _nextEvent = EventComputer.ComputeNext(this);
            }
        }

        public PlanarVector LocalPosition
        {
            get { return _orbitTransform.Orbit.ComputePosition(_orbitTransform.Angle); }
            set { Motion = new Orbit.Motion(value, LocalVelocity, _parent.Mass); }
        }

        public PlanarVector LocalVelocity
        {
            get { return _orbitTransform.Orbit.ComputeVelocity(_orbitTransform.Angle, _angleSpeed); }
            set { Motion = new Orbit.Motion(LocalPosition, value, _parent.Mass); }
        }

        public Path LocalPath
        {
            get
            {
                return _orbitTransform.LocalPath;
                /*if (_nextEvent == null) return _orbitTransform.LocalPath;
                if (_nextEvent.ExecutionTime >= Time.time + Motion.Period) return _orbitTransform.LocalPath;
                if (_path == null) _path = new Vector3[PATH_TOPOLOGY];
                var motion = new Orbit.Motion(_orbitTransform.Orbit, _orbitTransform.Angle, AngleSpeed);
                motion.BuildPath(Motion.ComputeAngle(_nextEvent.ExecutionTime), PLANAR_PATH_BUFFER);
                VectorConverter.Convert(PLANAR_PATH_BUFFER, _path);
                return new Path(_path, false);*/
            }
        }

        public double AngleSpeed { get { return _angleSpeed; } }

        [SerializeField]
        private OrbitTransform _orbitTransform;
        [SerializeField]
        private GravityField _parent;
        [SerializeField]
        private double _angleSpeed;
        [SerializeField]
        private EventComputer.Event _nextEvent;
        [NonSerialized]
        private Vector3[] _path;
        [NonSerialized]
        private bool _initialized;

        #region messages
        private void Awake()
        {
            Initialize();
        }

        private void Reset()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            _parent = transform.WrapParent().GetComponent<GravityField>();
            _orbitTransform = GetComponent<OrbitTransform>();
            _angleSpeed = _parent != null ? _orbitTransform.Orbit.ComputeAngleSpeed(_parent.Mass, _angleSpeed) : 1e-3;
            _orbitTransform.AngleChanged += OnAngleChanged;
        }

        private void OnEnable()
        {
            if (Time.time > 0) _nextEvent = EventComputer.ComputeNext(this);
        }

        private void FixedUpdate()
        {
            if (_parent == null) return;
            AngleChangedManually = true;
            _orbitTransform.Angle += _angleSpeed * Time.fixedDeltaTime;
            if (_nextEvent == null) return;
            if (_nextEvent.ExecutionTime < Time.time)
            {
                try
                {
                    _nextEvent.Execute(Time.time);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                _nextEvent = EventComputer.ComputeNext(this);
            }
        }

        private void OnTransformParentChanged()
        {
            _parent = transform.WrapParent().GetComponent<GravityField>();
            if (ParentChangedManually)
            {
                ParentChangedManually = false;
                return;
            }
            if (Application.isPlaying)
                Debug.LogWarningFormat(this, "Unhandled transform parent change, it's recommended to change parent through {0} methods.", GetType().Name);
            if (_parent == null)
            {
                _nextEvent = null;
                return;
            }
            _angleSpeed = _orbitTransform.Orbit.ComputeAngleSpeed(_parent.Mass, _angleSpeed);
            _nextEvent = EventComputer.ComputeNext(this);
        }

        private void OnDrawGizmos()
        {
            if (_parent == null) return;
            Gizmos.color = new Color(1, 1, 1, .05f);
            var path = LocalPath;
            var parent = transform.WrapParent();
            parent.TransformPoints(path.Curve, PointsBuffer);
            GizmosUtil.DrawPolyline(PointsBuffer, path.Enclosed);
        }

        private void OnParentGravityFieldInitialized(GravityField field)
        {
            _parent = field;
            UpdateOrbitFromParent();
        }

        private void OnParentGravityFieldModified()
        {
            UpdateOrbitFromParent();
        }

        private void UpdateOrbitFromParent()
        {
            if (Application.isPlaying)
                Motion = new Orbit.Motion(LocalPosition, LocalVelocity, _parent.Mass);
            else
            {
                _angleSpeed = _orbitTransform.Orbit.ComputeAngleSpeed(_parent.Mass, _angleSpeed);
                _nextEvent = EventComputer.ComputeNext(this);
            }
        }

        private void OnOrbitChanged(Orbit newOrbit)
        {
            if (OrbitChangedManually)
            {
                OrbitChangedManually = false;
                return;
            }
            if (_parent == null) return;
            _angleSpeed = newOrbit.ComputeAngleSpeed(_parent.Mass, _angleSpeed);
            _nextEvent = EventComputer.ComputeNext(this);
        }

        private void OnAngleChanged()
        {
            if (AngleChangedManually)
            {
                AngleChangedManually = false;
                return;
            }
            _nextEvent = EventComputer.ComputeNext(this);
        }

        private void OnDestroy()
        {
            _orbitTransform.AngleChanged -= OnAngleChanged;
        }
        #endregion messages

        private void ChangeParent(GravityField newParent, Orbit.Motion newMotion)
        {
            ParentChangedManually = true;
            _parent = newParent;
            transform.parent = newParent.transform;
            Motion = newMotion;
        }
    }
}