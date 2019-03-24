using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        private struct HyperbolicComputer : IComputer
        {
            private const float HYPERBOLIC_APOAPSIS = 1e+9f;

            public bool Periodic { get { return false; } }
            public double SemiMajorAxis { get { return _semiMajorAxis; } }
            public double Apoapsis { get { return double.MaxValue; } }
            public double Periapsis { get { return _periapsis; } }
            public double Argument { get { return _inclination.Angle + Math.PI; } }
            public double Eccentricity { get { return _eccentricity; } }
            public PlanarCircle Bounds { get { throw new InvalidOperationException("Hyperbolic bounds can not be approximated."); } }

            private double _semiMajorAxis;
            private double _periapsis;
            private double _eccentricity;
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
                _periapsis = periapsis;
                _eccentricity = eccentricity;
                _semiMajorAxis = periapsis / (1 - eccentricity);
                _xModificator = _semiMajorAxis * eccentricity;
                _yModificator = _semiMajorAxis * Math.Sqrt(eccentricity * eccentricity - 1);
                _inclination = new PlanarRotation(argument + Math.PI);
            }

            public IComputer Instantiate(PlanarVector position, PlanarVector velocity, double mass)
            {
                return new HyperbolicComputer(position, velocity, mass);
            }

            private HyperbolicComputer(PlanarVector position, PlanarVector velocity, double mass)
            {
                var eccVector = CalculateEccentricity(position, velocity, mass);
                _inclination = new PlanarRotation(Math.Atan2(-eccVector.Y, -eccVector.X));
                _eccentricity = eccVector.Magnitude;

                var v = velocity.Magnitude;
                var r = position.Magnitude;
                _semiMajorAxis = 1 / (2 / r - v * v / mass);
                _periapsis = _semiMajorAxis * (1 - _eccentricity);

                _xModificator = _semiMajorAxis * _eccentricity;
                _yModificator = _semiMajorAxis * Math.Sqrt(_eccentricity * _eccentricity - 1);
            }

            public IComputer Instantiate(PlanarVector position, double eccentricity, double argument)
            {
                return new HyperbolicComputer(position, eccentricity, argument);
            }

            // TODO: Peri-apsis can be negative. Add validation that gravity field will not intersect parent's surface.
            private HyperbolicComputer(PlanarVector position, double eccentricity, double argument)
            {
                _eccentricity = eccentricity;
                _inclination = new PlanarRotation(argument + Math.PI);
                position = _inclination.InvertedRotate(position);
                var r = position.Magnitude;
                _periapsis = (r - _eccentricity * position.X) / (1 + _eccentricity);
                _periapsis = Validator.ValidatePeriapsis(_periapsis);
                _semiMajorAxis = _periapsis / (1 - eccentricity);

                _xModificator = _semiMajorAxis * eccentricity;
                _yModificator = _semiMajorAxis * Math.Sqrt(eccentricity * eccentricity - 1);
            }
#endregion constructor

            public PlanarVector VelocityFromLocal(double localAnomaly, double meanVelocity)
            {
                var timeDerivative = (_eccentricity * Math.Cosh(localAnomaly) - 1) / meanVelocity;
                PlanarVector velocity;
                velocity.X = -_semiMajorAxis * Math.Sinh(localAnomaly) / timeDerivative;
                velocity.Y = _yModificator * Math.Cosh(localAnomaly) / timeDerivative;
                return _inclination.Rotate(velocity);
            }

            public PlanarVector PositionFromLocal(double localAnomaly)
            {
                PlanarVector position;
                position.X = _xModificator - _semiMajorAxis * Math.Cosh(localAnomaly);
                position.Y = _yModificator * Math.Sinh(localAnomaly);
                return _inclination.Rotate(position);
            }

            public double AltitudeFromLocal(double localAnomaly)
            {
                return _semiMajorAxis * (1 - _eccentricity * Math.Cosh(localAnomaly));
            }

            public double LocalFromPosition(PlanarVector position)
            {
                position = _inclination.InvertedRotate(position);
                var cosh = _eccentricity - position.X / _semiMajorAxis;
                var sinh = position.Y / _semiMajorAxis / Math.Sqrt(_eccentricity * _eccentricity - 1);
                return MathUtil.Atanh(sinh / cosh);
            }

            public double LocalFromAltitude(double altitude)
            {
                return MathUtil.Acosh((1 - altitude / _semiMajorAxis) / _eccentricity);
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
                    var function = meanAnomaly - _eccentricity * Math.Sinh(oldAnomaly) + oldAnomaly;
                    var derivative = _eccentricity * Math.Cosh(oldAnomaly) - 1;
                    newAnomaly = oldAnomaly + function / derivative;
                } while (Math.Abs(oldAnomaly - newAnomaly) > LOCAL_ANOMALY_ACCURACY);
                return newAnomaly;
            }

            public double MeanFromLocal(double localAnomaly)
            {
                return _eccentricity * Math.Sinh(localAnomaly) - localAnomaly;
            }
        }
    }
}