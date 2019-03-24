namespace Asteroids.Gravity
{
    partial class Orbit
    {
        private interface IComputer
        {
            bool Periodic { get; }
            double SemiMajorAxis { get; }
            double Apoapsis { get; }
            double Periapsis { get; }
            double Argument { get; }
            double Eccentricity { get; }
            PlanarCircle Bounds { get; }

            bool CanInstantiate(double eccentricity);
            IComputer Instantiate(double eccentricity, double periapsis, double argument);
            IComputer Instantiate(PlanarVector position, PlanarVector velocity, double mass);
            IComputer Instantiate(PlanarVector position, double eccentricity, double argument);

            PlanarVector VelocityFromLocal(double localAnomaly, double meanVelocity);
            PlanarVector PositionFromLocal(double localAnomaly);
            double AltitudeFromLocal(double localAnomaly);
            double LocalFromPosition(PlanarVector position);
            double LocalFromAltitude(double altitude);
            double LocalFromMean(double meanAnomaly);
            double MeanFromLocal(double localAnomaly);
        }
    }
}