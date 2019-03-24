using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        // TODO: Add validation inside computers.
        private static class Validator
        {
            private const double EccCircularMax = 0.001;
            private const double EccEllipticMax = 0.999;
            private const double EccHyperbolicMax = 1.001;

            private static readonly IComputer[] computers = { new CircularComputer(), new EllipticComputer(), new HyperbolicComputer() };           

            public static IComputer FindComputer(double eccentricity)
            {
                for (var i = 0; i < computers.Length; i++)
                    if (computers[i].CanInstantiate(eccentricity))
                        return computers[i];
                throw new Exception("Can not found valid orbit computer.");
            }

            public static double ValidatePeriapsis(double periapsis)
            {
                if (periapsis < 1e-3) return 1e-3;
                return periapsis;
            }

            public static double ValidateArgument(double argument)
            {
                return MathUtil.RepeatAngle(argument);
            }

            public static double ValidateEccentricity(double eccentricity)
            {
                if (eccentricity < EccCircularMax) return 0;
                if (eccentricity < 1)
                {
                    if (eccentricity > EccEllipticMax) return EccEllipticMax;
                }
                else
                {
                    if (eccentricity < EccHyperbolicMax) return EccHyperbolicMax;
                }
                return eccentricity;
            }

            public static void ValidateComponents(ref PlanarVector position, ref PlanarVector velocity)
            {
                var momentum = PlanarVector.Cross(position, velocity);
                if (Math.Abs(momentum) < 1e-6)
                {
                    position = ValidatePosition(position);
                    if (velocity.Magnitude < 1e-3) velocity = PlanarVector.Cross(position.Normalized) * 1e-3;
                    momentum = PlanarVector.Cross(position, velocity);
                    if (Math.Abs(momentum) < 1e-6)
                        velocity = PlanarRotation.Rotate(velocity, 2e-3);
                }
            }

            public static PlanarVector ValidatePosition(PlanarVector position)
            {
                if (position.Magnitude < 1e-3) return new PlanarVector(1e-3, 0);
                return position;
            }
        }
    }
}