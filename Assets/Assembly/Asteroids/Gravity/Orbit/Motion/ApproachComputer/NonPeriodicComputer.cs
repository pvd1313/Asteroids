using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        partial class Motion
        {
            partial class ApproachComputer
            {
                private class NonPeriodicComputer : IComputer
                {
                    private Motion _motionA;
                    private Motion _motionB;

                    public NonPeriodicComputer(Motion nonPeriodicA, Motion nonPeriodicB)
                    {
                        _motionA = nonPeriodicA;
                        _motionB = nonPeriodicB;
                    }

                    public double? FindNextApproachTime(double approachDistance)
                    {
                        var velocityA = _motionA.Orbit.ComputeVelocity(0, _motionA.AngleSpeed);
                        var velocityB = _motionB.Orbit.ComputeVelocity(0, _motionB.AngleSpeed);
                        var timeStep = approachDistance / Math.Max(velocityA.Magnitude, velocityB.Magnitude) / 5;
                        var time = .0;
                        var distanceRel = 1.0;
                        var prevDistance = .0;

                        for(var i = 0; i < maxSteps; i++, time += timeStep * distanceRel)
                        {
                            var posA = _motionA.ComputePosition(time);
                            var posB = _motionB.ComputePosition(time);
                            var distance = PlanarVector.Distance(posA, posB);
                            distanceRel = Math.Pow(distance / approachDistance, distancePow);
                            if (distance <= approachDistance) return time;
                            if (_motionA.ComputeAngle(time) * _motionA.AngleSpeed > 0)
                                if (_motionB.ComputeAngle(time) * _motionB.AngleSpeed > 0)
                                    if (distance == prevDistance) return null;
                            prevDistance = distance;
                        }
                        return null;
                    }
                }
            }
        }
    }
}