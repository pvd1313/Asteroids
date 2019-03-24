using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        partial class Motion
        {
            partial class ApproachComputer
            {
                private class PeriodicComputer : IComputer
                {
                    private Motion _motionA;
                    private Motion _motionB;

                    public PeriodicComputer(Motion periodicA, Motion periodicB)
                    {
                        _motionA = periodicA;
                        _motionB = periodicB;
                        if (_motionB.Period < _motionA.Period)
                            Util.Swap(ref _motionA, ref _motionB);
                    }

                    public double? TryFindNextApproachTime(double approachDistance)
                    {
                        var timeStep = ComputeTimeStep(_motionA, approachDistance);
                        var distance = approachDistance;
                        var distanceMul = 1.0;
                        var time = .0;

                        for (var i = 0; i <= MAX_STEPS; i++, time += timeStep * distanceMul)
                        {
                            var posA = _motionA.ComputePosition(time);
                            var posB = _motionB.ComputePosition(time);
                            distance = PlanarVector.Distance(posA, posB);
                            distanceMul = Math.Pow(distance / approachDistance, DISTANCE_POW);
                            if (distance <= approachDistance) return time;
                        }
                        return null;
                    }                   
                }
            }
        }
    }
}