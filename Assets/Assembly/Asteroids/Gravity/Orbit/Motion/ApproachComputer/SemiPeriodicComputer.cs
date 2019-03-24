using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        partial class Motion
        {
            partial class ApproachComputer
            {
                private class SemiPeriodicComputer : IComputer
                {
                    private Motion _motionA;
                    private Motion _motionB;

                    public SemiPeriodicComputer(Motion periodic, Motion nonPeriodic)
                    {
                        _motionA = periodic;
                        _motionB = nonPeriodic;
                    }

                    public double? FindNextApproachTime(double approachDistance)
                    {
                        var timeStep = ComputeTimeStep(_motionA, approachDistance);
                        var time = timeStep;
                        var maxDistance = _motionA.Orbit.Apoapsis + approachDistance;
                        var distanceRel = 1.0;

                        for(var i = 0; i < maxSteps; i++, time += timeStep * distanceRel)
                        {
                            var posA = _motionA.ComputePosition(time);
                            var posB = _motionB.ComputePosition(time);
                            var distance = PlanarVector.Distance(posA, posB);
                            distanceRel = Math.Pow(distance / approachDistance, distancePow);
                            if (distance <= approachDistance) return time;
                            if (_motionB.ComputeAngle(time) * _motionB.AngleSpeed > 0)
                                if (posB.Magnitude >= maxDistance)
                                    return null;
                        }
                        return null;
                    }
                }
            }
        }
    }
}