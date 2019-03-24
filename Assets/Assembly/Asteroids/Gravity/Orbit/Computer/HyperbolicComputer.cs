using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        private struct HyperbolicComputer : IComputer
        {
            private const float HyperbolicApoapsis = 1e+9f;

            public bool Periodic { get { return false; } }
            public double SemiMajorAxis { get; private set; }
            public double Apoapsis { get { return double.MaxValue; } }
            public double Periapsis { get; private set; }
            public double Argument { get { return _inclination.Angle + Math.PI; } }
            public double Eccentricity { get; private set; }
            public PlanarCircle Bounds { get { throw new InvalidOperationException("Hyperbolic bounds can not be approximated"); } }

            private PlanarRotation _inclination;
            private double _xModificator;
            private double _yModificator;

#region constructor
            public bool CanInstantiate(double eccentricity)
            {
                return eccentricity >= 1;
            }

            public IComputer Instantiate(double eccentricity, double periapsis, double argument)
            {
                return new HyperbolicComputer(eccentricity, periapsis, argument);
            }

            private HyperbolicComputer(double eccentricity, double periapsis, double argument)
            {
                Periapsis = periapsis;
                Eccentricity = eccentricity;
                SemiMajorAxis = periapsis / (1 - eccentricity);
                _xModificator = SemiMajorAxis * eccentricity;
                _yModificator = SemiMajorAxis * Math.Sqrt(eccentricity * eccentricity - 1);
                _inclination = new PlanarRotation(argument + Math.PI);
            }

            public IComputer Instantiate(PlanarVector position, PlanarVector velocity, double mass)
            {
                return new HyperbolicComputer(position, velocity, mass);
            }

            private HyperbolicComputer(PlanarVector position, PlanarVector velocity, double mass)
            {
                var eccVector = CalculateEccentricity(position, velocity, mass);
                _inclination = new PlanarRotation(Math.Atan2(-eccVector.y, -eccVector.x));
                Eccentricity = eccVector.Magnitude;

                var v = velocity.Magnitude;
                var r = position.Magnitude;
                SemiMajorAxis = 1 / (2 / r - v * v / mass);
                Periapsis = SemiMajorAxis * (1 - Eccentricity);

                _xModificator = SemiMajorAxis * Eccentricity;
                _yModificator = SemiMajorAxis * Math.Sqrt(Eccentricity * Eccentricity - 1);
            }

            public IComputer Instantiate(PlanarVector position, double eccentricity, double argument)
            {
                return new HyperbolicComputer(position, eccentricity, argument);
            }

            // TODO: Peri-apsis can be negative. Add validation that gravity field will not intersect parent's surface.
            private HyperbolicComputer(PlanarVector position, double eccentricity, double argument)
            {
                Eccentricity = eccentricity;
                _inclination = new PlanarRotation(argument + Math.PI);
                position = _inclination.InvertedRotate(position);
                var r = position.Magnitude;
                Periapsis = (r - Eccentricity * position.x) / (1 + Eccentricity);
                Periapsis = Validator.ValidatePeriapsis(Periapsis);
                SemiMajorAxis = Periapsis / (1 - eccentricity);

                _xModificator = SemiMajorAxis * eccentricity;
                _yModificator = SemiMajorAxis * Math.Sqrt(eccentricity * eccentricity - 1);
            }
#endregion constructor

            public PlanarVector VelocityFromLocal(double localAnomaly, double meanVelocity)
            {
                var timeDerivative = (Eccentricity * Math.Cosh(localAnomaly) - 1) / meanVelocity;
                PlanarVector velocity;
                velocity.x = -SemiMajorAxis * Math.Sinh(localAnomaly) / timeDerivative;
                velocity.y = _yModificator * Math.Cosh(localAnomaly) / timeDerivative;
                return _inclination.Rotate(velocity);
            }

            public PlanarVector PositionFromLocal(double localAnomaly)
            {
                PlanarVector position;
                position.x = _xModificator - SemiMajorAxis * Math.Cosh(localAnomaly);
                position.y = _yModificator * Math.Sinh(localAnomaly);
                return _inclination.Rotate(position);
            }

            public double AltitudeFromLocal(double localAnomaly)
            {
                return SemiMajorAxis * (1 - Eccentricity * Math.Cosh(localAnomaly));
            }

            public double LocalFromPosition(PlanarVector position)
            {
                position = _inclination.InvertedRotate(position);
                var cosh = Eccentricity - position.x / SemiMajorAxis;
                var sinh = position.y / SemiMajorAxis / Math.Sqrt(Eccentricity * Eccentricity - 1);
                return MathUtil.Atanh(sinh / cosh);
            }

            public double LocalFromAltitude(double altitude)
            {
                return MathUtil.Acosh((1 - altitude / SemiMajorAxis) / Eccentricity);
            }

            // TODO: Fix problem with computation limits (current 600).
            public double LocalFromMean(double meanAnomaly)
            {
                if (meanAnomaly < -600) meanAnomaly = -600;
                if (meanAnomaly > 600) meanAnomaly = 600;
                double newAnomaly = meanAnomaly;
                var iter = 0;
                double oldAnomaly;
                do
                {
                    if (iter > 16)
                    {
                        UnityEngine.Debug.LogWarning("Function not converged.");
                        break;
                    }
                    oldAnomaly = newAnomaly;
                    var function = meanAnomaly - Eccentricity * Math.Sinh(oldAnomaly) + oldAnomaly;
                    var derivative = Eccentricity * Math.Cosh(oldAnomaly) - 1;
                    newAnomaly = oldAnomaly + function / derivative;
                } while (Math.Abs(oldAnomaly - newAnomaly) > LocalAnomalyAccuracy);
                return newAnomaly;
            }

            public double MeanFromLocal(double localAnomaly)
            {
                return Eccentricity * Math.Sinh(localAnomaly) - localAnomaly;
            }
        }
    }
}