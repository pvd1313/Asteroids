using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        partial class Motion
        {
            private partial class ApproachComputer
            {
                public static readonly int maxSteps = 1000;
                public static readonly double distancePow = 1.5;

                public interface IComputer
                {
                    double? FindNextApproachTime(double approachDistance);
                }

                public static IComputer Create(Motion motionA, Motion motionB)
                {
                    var periodicA = motionA.Orbit.Periodic;
                    var periodicB = motionB.Orbit.Periodic;

                    if (periodicA)
                    {
                        if (!periodicB) return new SemiPeriodicComputer(motionA, motionB);
                        return new PeriodicComputer(motionA, motionB);
                    }
                    if (periodicB) return new SemiPeriodicComputer(motionB, motionA);
                    return new NonPeriodicComputer(motionA, motionB);
                }

                private static double ComputeTimeStep(Motion motion, double distanceStep)
                {
                    var motionBounds = motion.Orbit.Bounds;
                    var topology = motionBounds.Perimeter / distanceStep;
                    return motion.Period / (int)Math.Max(topology, 16);
                }
            }
        }
    }
}
