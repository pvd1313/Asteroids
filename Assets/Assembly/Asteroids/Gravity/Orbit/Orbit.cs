using UnityEngine;
using System;

namespace Asteroids.Gravity
{
    [Serializable]
    public partial class Orbit : ISerializationCallbackReceiver
    {
        private const double LocalAnomalyAccuracy = 1e-9;

        public double Eccentricity { get { return _eccentricity; } }
        public double Apoapsis { get { return _computer.Apoapsis; } }
        public double Periapsis { get { return _periapsis; } }
        public double Argument { get { return _argument; } }
        public PlanarVector Apocenter { get { return PlanarRotation.Rotate(_computer.Apoapsis, _argument + Math.PI); } }
        public PlanarVector Pericenter { get { return PlanarRotation.Rotate(_computer.Periapsis, _argument); } }
        public bool Periodic { get { return _computer.Periodic; } }

        /// <summary>
        /// Exception when hyperbolic.
        /// </summary>
        private PlanarCircle Bounds { get { return _computer.Bounds; } }
        [SerializeField]
        private double _eccentricity;
        [SerializeField]
        private double _periapsis;
        [SerializeField]
        private double _argument;
        [NonSerialized]
        private IComputer _computer;

    #region static
        public static Orbit CreateSimple(PlanarVector position)
        {
            return new Orbit(0, position.Magnitude, 0);
        }   

        private static PlanarVector CalculateEccentricity(PlanarVector position, PlanarVector velocity, double mass)
        {
            var angularMomentum = PlanarVector.Cross(position, velocity);
            var r = position.Magnitude;
            var momentumMass = angularMomentum / mass;
            PlanarVector eccentricity;
            eccentricity.x =  velocity.y * momentumMass - position.x / r;
            eccentricity.y = -velocity.x * momentumMass - position.y / r;
            return eccentricity;
        }

        private static IComputer CreateComputer(double eccentricity, double periapsis, double argument)
        {
            var computer = Validator.FindComputer(eccentricity);
            return computer.Instantiate(eccentricity, periapsis, argument);
        }
    #endregion static

        public Orbit(double eccentricity, double periapsis, double argument)
        {
            _eccentricity = Validator.ValidateEccentricity(eccentricity);
            _periapsis = Validator.ValidatePeriapsis(periapsis);
            _argument = Validator.ValidateArgument(argument);
            _computer = CreateComputer(_eccentricity, _periapsis, _argument);
        }

        private Orbit(PlanarVector position, PlanarVector velocity, double mass)
        {
            var eccentricity = CalculateEccentricity(position, velocity, mass).Magnitude;
            eccentricity = Validator.ValidateEccentricity(eccentricity);
            var computer = Validator.FindComputer(eccentricity);
            _computer = computer.Instantiate(position, velocity, mass);
            InitializeFrom(_computer);
        }

        private Orbit(PlanarVector position, double eccentricity, double argument)
        {
            var computer = Validator.FindComputer(eccentricity);
            _computer = computer.Instantiate(position, eccentricity, argument);
            InitializeFrom(_computer);
        }

        private void InitializeFrom(IComputer computer)
        {
            _eccentricity = computer.Eccentricity;
            _periapsis = computer.Periapsis;
            _argument = computer.Argument;
        }

        public double ComputeAngleSpeed(double mass, double sign)
        {
            var a = Math.Abs(_computer.SemiMajorAxis);
            var speed = Math.Sqrt(mass / (a * a * a));
            if (sign >= 0) return speed;
            return -speed;
        }

        public PlanarVector ComputeVelocity(double angle, double angleSpeed)
        {
            var localAnomaly = _computer.LocalFromMean(angle);
            return _computer.VelocityFromLocal(localAnomaly, angleSpeed);
        }

        public PlanarVector ComputePosition(double angle)
        {
            return _computer.PositionFromLocal(_computer.LocalFromMean(angle));
        }

        public double ComputeAltitude(double angle)
        {
            return _computer.AltitudeFromLocal(_computer.LocalFromMean(angle));
        }

        public double ComputeAngle(PlanarVector position)
        {
            return _computer.MeanFromLocal(_computer.LocalFromPosition(position));
        }

        /// <summary>
        /// Exception when circular.
        /// </summary>
        public double ComputeAngle(double altitude)
        {
            return _computer.MeanFromLocal(_computer.LocalFromAltitude(altitude));
        }

        public void BuildPath(PlanarVector[] result)
        {
            BuildPathOnLocalAnomaly(-Math.PI, 2 * Math.PI, result);
        }

        private void BuildPathOnLocalAnomaly(double angle, double delta, PlanarVector[] result)
        { 
            if (result.Length > 0)
            {
                delta /= (result.Length - 1);
                for (var i = 0; i < result.Length; i++, angle += delta)
                    result[i] = _computer.PositionFromLocal(angle);
            }
        }

        private double ComputeMass(double angleSpeed)
        {
            var a = Math.Abs(_computer.SemiMajorAxis);
            return angleSpeed * angleSpeed * a * a * a;
        }

        /// <summary>
        /// False when circular.
        /// </summary>
        private bool IntersectsAltitude(double altitude)
        {
            return (_periapsis <= altitude) && (_computer.Apoapsis >= altitude);
        }

        private bool IntersectsOrbit(Orbit target, double maxDistance)
        {
            if (Periodic)
            {
                if (!target.Periodic) return target._periapsis <= Apoapsis + maxDistance;
                return Bounds.ComputeBorderDistance(target.Bounds) <= maxDistance;
            }
            if (target.Periodic) return _periapsis <= target.Apoapsis + maxDistance;
            return true;
        }

    #region serialization
        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Validate();
            _computer = CreateComputer(_eccentricity, _periapsis, _argument);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Validate();
            _computer = CreateComputer(_eccentricity, _periapsis, _argument);
        }
    #endregion

        private void Validate()
        {
            _eccentricity = Validator.ValidateEccentricity(_eccentricity);
            _periapsis = Validator.ValidatePeriapsis(_periapsis);
            _argument = Validator.ValidateArgument(_argument);
        }
    }
}