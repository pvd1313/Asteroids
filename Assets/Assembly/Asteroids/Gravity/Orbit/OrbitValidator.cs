using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        // TODO: Add validation inside computers.
        private static class Validator
        {
            private static readonly IComputer[] COMPUTERS = { new CircularComputer(), new EllipticComputer(), new HyperbolicComputer() };
            private const double ECC_CIRCULAR_MAX   = 0.001;
            private const double ECC_ELLIPTIC_MAX   = 0.999;
            private const double ECC_HYPERBOLIC_MIN = 1.001;

            public static IComputer FindComputer(double eccentricity)
            {
                for (var i = 0; i < COMPUTERS.Length; i++)
                    if (COMPUTERS[i].CanInstantiate(eccentricity))
                        return COMPUTERS[i];
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
                if (eccentricity < ECC_CIRCULAR_MAX) return 0;
                if (eccentricity < 1)
                {
                    if (eccentricity > ECC_ELLIPTIC_MAX) return ECC_ELLIPTIC_MAX;
                }
                else
                {
                    if (eccentricity < ECC_HYPERBOLIC_MIN) return ECC_HYPERBOLIC_MIN;
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