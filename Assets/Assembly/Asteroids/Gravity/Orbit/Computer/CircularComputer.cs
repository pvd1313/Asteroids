using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        private struct CircularComputer : IComputer
        {
            public bool Periodic { get { return true; } }
            public double SemiMajorAxis { get; private set; }
            public double Apoapsis { get { return SemiMajorAxis; } }
            public double Periapsis { get { return SemiMajorAxis; } }
            public double Argument { get; private set; }
            public double Eccentricity { get { return 0; } }
            public PlanarCircle Bounds { get; private set; }

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
                SemiMajorAxis = periapsis;
                Bounds = new PlanarCircle(PlanarVector.zero, SemiMajorAxis);
                Argument = argument;
            }

            public IComputer Instantiate(PlanarVector position, PlanarVector velocity, double mass)
            {
                return new CircularComputer(position, velocity, mass);
            }

            private CircularComputer(PlanarVector position, PlanarVector velocity, double mass)
            {
                SemiMajorAxis = position.Magnitude;
                Bounds = new PlanarCircle(PlanarVector.zero, SemiMajorAxis);
                Argument = 0;
            }

            public IComputer Instantiate(PlanarVector position, double eccentricity, double argument)
            {
                return new CircularComputer(position, eccentricity, argument);
            }

            private CircularComputer(PlanarVector position, double eccentricity, double argument)
            {
                SemiMajorAxis = position.Magnitude;
                Bounds = new PlanarCircle(PlanarVector.zero, SemiMajorAxis);
                Argument = argument;
            }
#endregion constructor

            public PlanarVector VelocityFromLocal(double localAnomaly, double meanVelocity)
            {
                var angle = localAnomaly + Argument;
                PlanarVector velocity;
                velocity.x = -SemiMajorAxis * Math.Sin(angle) * meanVelocity;
                velocity.y =  SemiMajorAxis * Math.Cos(angle) * meanVelocity;
                return velocity;
            }

            public PlanarVector PositionFromLocal(double localAnomaly)
            {
                var angle = localAnomaly + Argument;
                PlanarVector position;
                position.x = SemiMajorAxis * Math.Cos(angle);
                position.y = SemiMajorAxis * Math.Sin(angle);
                return position;
            }

            public double AltitudeFromLocal(double localAnomaly)
            {
                return SemiMajorAxis;
            }

            public double LocalFromPosition(PlanarVector position)
            {
                return Math.Atan2(position.y, position.x) - Argument;
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