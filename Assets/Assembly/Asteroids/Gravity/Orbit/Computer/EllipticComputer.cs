using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        private struct EllipticComputer : IComputer
        {
            public bool Periodic { get { return true; } }
            public double SemiMajorAxis { get; private set; }
            public double Apoapsis { get; private set; }
            public double Periapsis { get; private set; }
            public double Argument { get { return _inclination.Angle; } }
            public double Eccentricity { get; private set; }
            public PlanarCircle Bounds { get; private set; }

            private double _semiMinorAxis;
            private PlanarRotation _inclination;

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
                Eccentricity = eccentricity;
                Periapsis = periapsis;
                SemiMajorAxis = Periapsis / (1 - Eccentricity);
                _semiMinorAxis = SemiMajorAxis * Math.Sqrt(1 - Eccentricity * Eccentricity);
                Apoapsis = SemiMajorAxis * (1 + Eccentricity);
                Bounds = new PlanarCircle(_inclination.Rotate(-SemiMajorAxis * Eccentricity), SemiMajorAxis);
            }

            public IComputer Instantiate(PlanarVector position, PlanarVector velocity, double mass)
            {
                return new EllipticComputer(position, velocity, mass);
            }

            private EllipticComputer(PlanarVector position, PlanarVector velocity, double mass)
            {
                var eccVector = CalculateEccentricity(position, velocity, mass);
                Eccentricity = eccVector.Magnitude;
                var eSqrInverted = 1 - Eccentricity * Eccentricity;

                var angularMomentum = PlanarVector.Cross(position, velocity);
                SemiMajorAxis = angularMomentum * angularMomentum / mass / eSqrInverted;
                _semiMinorAxis = SemiMajorAxis * Math.Sqrt(eSqrInverted);
                Apoapsis = SemiMajorAxis * (1 + Eccentricity);
                Periapsis = SemiMajorAxis * (1 - Eccentricity);
                _inclination = new PlanarRotation(Math.Atan2(eccVector.y, eccVector.x));
                Bounds = new PlanarCircle(_inclination.Rotate(-SemiMajorAxis * Eccentricity), SemiMajorAxis);
            }

            public IComputer Instantiate(PlanarVector position, double eccentricity, double argument)
            {
                return new EllipticComputer(position, eccentricity, argument);
            }

            private EllipticComputer(PlanarVector position, double eccentricity, double argument)
            {
                Eccentricity = eccentricity;
                _inclination = new PlanarRotation(argument);
                position = _inclination.InvertedRotate(position);
                var r = position.Magnitude;
                Periapsis = (r + Eccentricity * position.x) / (1 + Eccentricity);
                SemiMajorAxis = Periapsis / (1 - Eccentricity);
                _semiMinorAxis = SemiMajorAxis * Math.Sqrt(1 - Eccentricity * Eccentricity);
                Apoapsis = SemiMajorAxis * (1 + Eccentricity);
                Bounds = new PlanarCircle(_inclination.Rotate(-SemiMajorAxis * Eccentricity), SemiMajorAxis);
            }
#endregion constructor

            public PlanarVector VelocityFromLocal(double localAnomaly, double meanVelocity)
            {
                var timeDerivative = (1 - Eccentricity * Math.Cos(localAnomaly)) / meanVelocity;
                PlanarVector velocity;
                velocity.x = -SemiMajorAxis * Math.Sin(localAnomaly) / timeDerivative;
                velocity.y = _semiMinorAxis * Math.Cos(localAnomaly) / timeDerivative;
                return _inclination.Rotate(velocity);
            }

            public PlanarVector PositionFromLocal(double localAnomaly)
            {
                PlanarVector position;
                position.x = SemiMajorAxis * (Math.Cos(localAnomaly) - Eccentricity);
                position.y = Math.Sin(localAnomaly) * _semiMinorAxis;
                return _inclination.Rotate(position);
            }

            public double AltitudeFromLocal(double localAnomaly)
            {
                return SemiMajorAxis * (1 - Eccentricity * Math.Cos(localAnomaly));
            }

            public double LocalFromPosition(PlanarVector position)
            {
                position = _inclination.InvertedRotate(position);
                var eccentricitySin = position.y / _semiMinorAxis;
                var eccentricityCos = position.x / SemiMajorAxis + Eccentricity;
                return Math.Atan2(eccentricitySin, eccentricityCos);
            }

            public double LocalFromAltitude(double altitude)
            {
                return Math.Acos((1 - altitude / SemiMajorAxis) / Eccentricity);
            }

            public double LocalFromMean(double meanAnomaly)
            {
                var iter = 0;
                meanAnomaly = MathUtil.RepeatAngle(meanAnomaly);
                double newAnomaly = Eccentricity > .8 ? Math.PI : meanAnomaly;
                double oldAnomaly;
                do
                {
                    if (iter > 16)
                    {
                        UnityEngine.Debug.LogWarning("Function not converged.");
                        return 0;
                    }
                    oldAnomaly = newAnomaly;
                    var function = oldAnomaly - Eccentricity * Math.Sin(oldAnomaly) - meanAnomaly;
                    var derivative = 1 - Eccentricity * Math.Cos(oldAnomaly);
                    newAnomaly = oldAnomaly - function / derivative;
                    iter++;
                } while (Math.Abs(oldAnomaly - newAnomaly) > LocalAnomalyAccuracy);
                return newAnomaly;
            }

            public double MeanFromLocal(double localAnomaly)
            {
                return localAnomaly - Math.Sin(localAnomaly) * Eccentricity;
            }
        }
    }
}