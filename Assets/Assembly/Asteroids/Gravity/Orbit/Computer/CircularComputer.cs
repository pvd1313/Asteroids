using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        private struct CircularComputer : IComputer
        {
            public bool Periodic { get { return true; } }
            public double SemiMajorAxis { get { return _semiAxis; } }
            public double Apoapsis { get { return _semiAxis; } }
            public double Periapsis { get { return _semiAxis; } }
            public double Argument { get { return _inclination; } }
            public double Eccentricity { get { return 0; } }
            public PlanarCircle Bounds { get { return _bounds; } }

            private double _semiAxis;
            private double _inclination;
            private PlanarCircle _bounds;

#region constructor
            public bool CanInstantiate(double eccentricity)
            {
                return eccentricity == 0;
            }

            public IComputer Instantiate(double eccentricity, double periapsis, double argument)
            {
                return new CircularComputer(eccentricity, periapsis, argument);
            }

            private CircularComputer(double eccentricity, double periapsis, double argument)
            {
                _semiAxis = periapsis;
                _bounds = new PlanarCircle(PlanarVector.Zero, _semiAxis);
                _inclination = argument;
            }

            public IComputer Instantiate(PlanarVector position, PlanarVector velocity, double mass)
            {
                return new CircularComputer(position, velocity, mass);
            }

            private CircularComputer(PlanarVector position, PlanarVector velocity, double mass)
            {
                _semiAxis = position.Magnitude;
                _bounds = new PlanarCircle(PlanarVector.Zero, _semiAxis);
                _inclination = 0;
            }

            public IComputer Instantiate(PlanarVector position, double eccentricity, double argument)
            {
                return new CircularComputer(position, eccentricity, argument);
            }

            private CircularComputer(PlanarVector position, double eccentricity, double argument)
            {
                _semiAxis = position.Magnitude;
                _bounds = new PlanarCircle(PlanarVector.Zero, _semiAxis);
                _inclination = argument;
            }
#endregion constructor

            public PlanarVector VelocityFromLocal(double localAnomaly, double meanVelocity)
            {
                var angle = localAnomaly + _inclination;
                PlanarVector velocity;
                velocity.X = -_semiAxis * Math.Sin(angle) * meanVelocity;
                velocity.Y =  _semiAxis * Math.Cos(angle) * meanVelocity;
                return velocity;
            }

            public PlanarVector PositionFromLocal(double localAnomaly)
            {
                var angle = localAnomaly + _inclination;
                PlanarVector position;
                position.X = _semiAxis * Math.Cos(angle);
                position.Y = _semiAxis * Math.Sin(angle);
                return position;
            }

            public double AltitudeFromLocal(double localAnomaly)
            {
                return _semiAxis;
            }

            public double LocalFromPosition(PlanarVector position)
            {
                return Math.Atan2(position.Y, position.X) - _inclination;
            }

            public double LocalFromAltitude(double altitude)
            {
                throw new InvalidOperationException();
            }

            public double LocalFromMean(double meanAnomaly)
            {
                return meanAnomaly;
            }

            public double MeanFromLocal(double localAnomaly)
            {
                return localAnomaly;
            }
        }
    }
}