using System;

namespace Asteroids.Gravity
{
    partial class Orbit
    {
        public partial class Motion
        {
            public Orbit Orbit { get; private set; }
            public double Angle { get; private set; }
            public double AngleSpeed { get; private set; }
            public bool Periodic { get { return Orbit.Periodic; } }
            public double Period
            {
                get
                {
                    if (!Periodic) return double.PositiveInfinity;
                    return Math.Abs(MathUtil.TWO_PI / AngleSpeed);
                }
            }

            public Motion(PlanarVector position, PlanarVector velocity, double mass)
            {
                if (mass <= 0) throw new ArgumentException("Mass must be greater than zero.");
                Validator.ValidateComponents(ref position, ref velocity);

                Orbit = new Orbit(position, velocity, mass);
                Angle = Orbit.ComputeAngle(position);
                AngleSpeed = Orbit.ComputeAngleSpeed(mass, PlanarVector.Cross(position, velocity));
            }

            public Motion(Orbit orbit, double angle, double angleSpeed)
            {
                if (orbit == null) throw new ArgumentException("Orbit can not be null.");
                if (angleSpeed == 0) throw new ArgumentException("Angle speed can not be zero.");

                Orbit = orbit;
                Angle = orbit.Periodic ? MathUtil.RepeatAngle(angle) : angle;
                AngleSpeed = angleSpeed;
            }

            public Motion(Motion motion, double deltaTime)
            {
                if (motion == null) throw new ArgumentException("Motion can not be null.");
                Orbit = motion.Orbit;
                Angle = motion.ComputeAngle(deltaTime);
                AngleSpeed = motion.AngleSpeed;
            }

            public void BuildPath(double targetAngle, PlanarVector[] result)
            {
                var computer = Orbit._computer;
                var start = computer.LocalFromMean(Angle);
                var end   = computer.LocalFromMean(targetAngle);
                var delta = MathUtil.DeltaAngle(start, end, AngleSpeed);
                Orbit.BuildPathOnLocalAnomaly(start, delta, result);
            }

            public Motion ComputeRelativeMotion(PlanarVector relativePosition)
            {
                var position = Orbit.ComputePosition(Angle) + relativePosition;
                position = Validator.ValidatePosition(position);
                var newOrbit = new Orbit(position, Orbit.Eccentricity, Orbit.Argument);
                return new Motion()
                {
                    Orbit = newOrbit,
                    Angle = newOrbit.ComputeAngle(position),
                    AngleSpeed = newOrbit.ComputeAngleSpeed(Orbit.ComputeMass(AngleSpeed), AngleSpeed)
                };
            }

            public double ComputeAngle(double deltaTime)
            {
                return Angle + deltaTime * AngleSpeed;
            }

            public PlanarVector ComputePosition(double deltaTime)
            {
                return Orbit.ComputePosition(Angle + deltaTime * AngleSpeed);
            }

            public PlanarVector ComputeVelocity(double deltaTime)
            {
                return Orbit.ComputeVelocity(Angle + deltaTime * AngleSpeed, AngleSpeed);
            }

            public double? TryPredictTimeWhenApproach(Motion target, double distance)
            {
                if (Orbit.IntersectsOrbit(target.Orbit, distance))
                {
                    var computer = ApproachComputer.Create(this, target);
                    var approach = computer.TryFindNextApproachTime(distance);
                    if (approach != null)
                    {
                        UnityEngine.Debug.LogFormat("Approach in {0}s", approach.Value);
                    }
                    else
                    {
                        UnityEngine.Debug.LogFormat("Approach not found!");
                    }
                    return approach;
                }
                return null;
            }

            public double? TryPredictTimeWhenAbove(double targetAltitude)
            {
                var altitude = Orbit.ComputeAltitude(Angle);
                if (altitude >= targetAltitude) return 0;
                return TryFindApproachTime(targetAltitude);
            }

            public double? TryPredictTimeWhenBelow(double targetAltitude)
            {
                var altitude = Orbit.ComputeAltitude(Angle);
                if (altitude <= targetAltitude) return 0;
                return TryFindApproachTime(targetAltitude);
            }

            private double? TryFindApproachTime(double targetAltitude)
            {
                if (Orbit.IntersectsAltitude(targetAltitude))
                {
                    var approachTime = ApproachTimeFromAltitude(targetAltitude);
                    if (approachTime >= 0) return approachTime;
                }
                return null;
            }

            private double ApproachTimeFromAltitude(double targetAltitude)
            {
                var periodic = Orbit.Periodic;

                var angle_0 = Orbit.ComputeAngle(targetAltitude);
                var angle_1 = periodic ? (MathUtil.TWO_PI - angle_0) : (-angle_0);
                var time_0 = ApproachTimeFromAngle(angle_0);
                var time_1 = ApproachTimeFromAngle(angle_1);

                if (periodic) return Math.Min(time_0, time_1);
                if (time_0 < time_1) return time_0 < 0 ? time_1 : time_0;
                return time_1 < 0 ? time_0 : time_1;
            }

            private double ApproachTimeFromAngle(double targetAngle)
            {
                if (Orbit.Periodic)
                {
                    targetAngle = MathUtil.RepeatAngle(targetAngle);
                    if (AngleSpeed > 0)
                        return PositiveDeltaAngle(Angle, targetAngle) / AngleSpeed;
                    return NegativeDeltaAngle(Angle, targetAngle) / AngleSpeed;
                }
                else
                    return (targetAngle - Angle) / AngleSpeed;
            }

            private static double PositiveDeltaAngle(double start, double end)
            {
                if (end >= start) return end - start;
                return end + MathUtil.TWO_PI - start;
            }

            private static double NegativeDeltaAngle(double start, double end)
            {
                if (start >= end) return -(start - end);
                return -(start + MathUtil.TWO_PI - end);
            }

            private Motion() { }
        }
    }
}