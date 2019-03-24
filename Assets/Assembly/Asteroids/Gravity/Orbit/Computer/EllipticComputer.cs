using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        private struct EllipticComputer : IComputer
        {
            public bool Periodic { get { return true; } }
            public double SemiMajorAxis { get { return _semiMajorAxis; } }
            public double Apoapsis { get { return _apoapsis; } }
            public double Periapsis { get { return _periapsis; } }
            public double Argument { get { return _inclination.Angle; } }
            public double Eccentricity { get { return _eccentricity; } }
            public PlanarCircle Bounds { get { return _bounds; } }

            private double _semiMinorAxis;
            private double _semiMajorAxis;
            private double _eccentricity;
            private double _apoapsis;
            private double _periapsis;
            private PlanarRotation _inclination;
            private PlanarCircle _bounds;

#region constructor
            public bool CanInstantiate(double eccentricity)
            {
                return (eccentricity < 1) && (eccentricity > 0);
            }

            public IComputer Instantiate(double eccentricity, double periapsis, double argument)
            {
                return new EllipticComputer(eccentricity, periapsis, argument);
            }

            private EllipticComputer(double eccentricity, double periapsis, double argument)
            {
                _inclination = new PlanarRotation(argument);
                _eccentricity = eccentricity;
                _periapsis = periapsis;
                _semiMajorAxis = _periapsis / (1 - _eccentricity);
                _semiMinorAxis = _semiMajorAxis * Math.Sqrt(1 - _eccentricity * _eccentricity);
                _apoapsis = _semiMajorAxis * (1 + _eccentricity);
                _bounds = new PlanarCircle(_inclination.Rotate(-_semiMajorAxis * _eccentricity), _semiMajorAxis);
            }

            public IComputer Instantiate(PlanarVector position, PlanarVector velocity, double mass)
            {
                return new EllipticComputer(position, velocity, mass);
            }

            private EllipticComputer(PlanarVector position, PlanarVector velocity, double mass)
            {
                var eccVector = CalculateEccentricity(position, velocity, mass);
                _eccentricity = eccVector.Magnitude;
                var eSqrInverted = 1 - _eccentricity * _eccentricity;

                var angularMomentum = PlanarVector.Cross(position, velocity);
                _semiMajorAxis = angularMomentum * angularMomentum / mass / eSqrInverted;
                _semiMinorAxis = _semiMajorAxis * Math.Sqrt(eSqrInverted);
                _apoapsis = _semiMajorAxis * (1 + _eccentricity);
                _periapsis = _semiMajorAxis * (1 - _eccentricity);
                _inclination = new PlanarRotation(Math.Atan2(eccVector.Y, eccVector.X));
                _bounds = new PlanarCircle(_inclination.Rotate(-_semiMajorAxis * _eccentricity), _semiMajorAxis);
            }

            public IComputer Instantiate(PlanarVector position, double eccentricity, double argument)
            {
                return new EllipticComputer(position, eccentricity, argument);
            }

            private EllipticComputer(PlanarVector position, double eccentricity, double argument)
            {
                _eccentricity = eccentricity;
                _inclination = new PlanarRotation(argument);
                position = _inclination.InvertedRotate(position);
                var r = position.Magnitude;
                _periapsis = (r + _eccentricity * position.X) / (1 + _eccentricity);
                _semiMajorAxis = _periapsis / (1 - _eccentricity);
                _semiMinorAxis = _semiMajorAxis * Math.Sqrt(1 - _eccentricity * _eccentricity);
                _apoapsis = _semiMajorAxis * (1 + _eccentricity);
                _bounds = new PlanarCircle(_inclination.Rotate(-_semiMajorAxis * _eccentricity), _semiMajorAxis);
            }
#endregion constructor

            public PlanarVector VelocityFromLocal(double localAnomaly, double meanVelocity)
            {
                var timeDerivative = (1 - _eccentricity * Math.Cos(localAnomaly)) / meanVelocity;
                PlanarVector velocity;
                velocity.X = -_semiMajorAxis * Math.Sin(localAnomaly) / timeDerivative;
                velocity.Y = _semiMinorAxis * Math.Cos(localAnomaly) / timeDerivative;
                return _inclination.Rotate(velocity);
            }

            public PlanarVector PositionFromLocal(double localAnomaly)
            {
                PlanarVector position;
                position.X = _semiMajorAxis * (Math.Cos(localAnomaly) - _eccentricity);
                position.Y = Math.Sin(localAnomaly) * _semiMinorAxis;
                return _inclination.Rotate(position);
            }

            public double AltitudeFromLocal(double localAnomaly)
            {
                return _semiMajorAxis * (1 - _eccentricity * Math.Cos(localAnomaly));
            }

            public double LocalFromPosition(PlanarVector position)
            {
                position = _inclination.InvertedRotate(position);
                var eccentricitySin = position.Y / _semiMinorAxis;
                var eccentricityCos = position.X / _semiMajorAxis + _eccentricity;
                return Math.Atan2(eccentricitySin, eccentricityCos);
            }

            public double LocalFromAltitude(double altitude)
            {
                return Math.Acos((1 - altitude / _semiMajorAxis) / _eccentricity);
            }

            public double LocalFromMean(double meanAnomaly)
            {
                var iter = 0;
                meanAnomaly = MathUtil.RepeatAngle(meanAnomaly);
                double newAnomaly = _eccentricity > .8 ? Math.PI : meanAnomaly;
                double oldAnomaly;
                do
                {
                    if (iter > 16)
                    {
                        UnityEngine.Debug.LogWarning("Function not converged.");
                        return 0;
                    }
                    oldAnomaly = newAnomaly;
                    var function = oldAnomaly - _eccentricity * Math.Sin(oldAnomaly) - meanAnomaly;
                    var derivative = 1 - _eccentricity * Math.Cos(oldAnomaly);
                    newAnomaly = oldAnomaly - function / derivative;
                    iter++;
                } while (Math.Abs(oldAnomaly - newAnomaly) > LOCAL_ANOMALY_ACCURACY);
                return newAnomaly;
            }

            public double MeanFromLocal(double localAnomaly)
            {
                return localAnomaly - Math.Sin(localAnomaly) * _eccentricity;
            }
        }
    }
}