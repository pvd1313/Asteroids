using NUnit.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Asteroids.Gravity.Test
{
    public class OrbitTest
    {
        private delegate void TestFunction(Orbit orbit);

        private const double MIN_VALUE = 1e-3 * Math.E;
        private const double POW_DELTA = 6;
        private const double TOLERANCE = 1e-3;
        private const int PERIAPSIS_COUNT = 6;
        private const int ARGUMENT_COUNT = 6;
        private const int MEAN_COUNT = 6;
        private const int MASS_COUNT = 6;
        private static readonly Orbit[] _circular = GenerateMulti(0);
        private static readonly Orbit[] _elliptic = GenerateMulti(0.0034, 0.059, 0.199, 0.259, 0.466, 0.567, 0.786, 0.963, 0.999, 0.99991);
        private static readonly Orbit[] _hyperbolic = GenerateMulti(1.0034, 1.059, 1.199, 1.259, 1.466, 2.567, 3.786, 5.963, 9.999, 15.4532, 36.2, 79.521);
        private static readonly FieldInfo ORBIT_COMPUTER_FIELD = typeof(Orbit).GetField("_computer", BindingFlags.Instance | BindingFlags.NonPublic);

        public class Circular
        {
            [Test] public void MeanPositionConversion() { RunMeanPositionConversion(_circular); }
            [Test] public void AbsoluteVelocity() { RunAbsoluteVelocity(_circular); }
            [Test] public void DynamicStaticConversion() { RunDynamicStaticConversion(_circular); }
        }

        public class Elliptic
        {
            [Test] public void MeanPositionConversion() { RunMeanPositionConversion(_elliptic); }
            [Test] public void AbsoluteVelocity() { RunAbsoluteVelocity(_elliptic); }
            [Test] public void DynamicStaticConversion() { RunDynamicStaticConversion(_elliptic); }
            [Test] public void AltitudeMeanConversion() { RunAltitudeMeanConversion(_elliptic); }
        }

        public class Hyperbolic
        {
            [Test] public void MeanPositionConversion() { RunMeanPositionConversion(_hyperbolic); }
            [Test] public void AbsoluteVelocity() { RunAbsoluteVelocity(_hyperbolic); }
            [Test] public void DynamicStaticConversion() { RunDynamicStaticConversion(_hyperbolic); }
            [Test] public void AltitudeMeanConversion() { RunAltitudeMeanConversion(_hyperbolic); }
        }

        private static Orbit[] GenerateMulti(params double[] e)
        {
            var result = new List<Orbit>(e.Length * PERIAPSIS_COUNT * ARGUMENT_COUNT);
            for (var i = 0; i < e.Length; i++)
                result.AddRange(Generate(e[i]));
            return result.ToArray();
        }

        private static Orbit[] Generate(double e)
        {
            var tests = new Orbit[PERIAPSIS_COUNT * ARGUMENT_COUNT];
            for (var p = 0; p < PERIAPSIS_COUNT; p++)
            {
                var periapsis = Pow(p, PERIAPSIS_COUNT);
                for (var a = 0; a < ARGUMENT_COUNT; a++)
                {
                    var argument = Pow(a, ARGUMENT_COUNT);
                    tests[p * PERIAPSIS_COUNT + a] = new Orbit(e, periapsis, argument);
                }
            }

            return tests;
        }

        private static void RunMeanPositionConversion(Orbit[] trs)
        {
            foreach (var tr in trs)
                for (var m = 0; m < MEAN_COUNT; m++)
                {
                    var mean = Pow(m, MEAN_COUNT);
                    var pos = tr.ComputePosition(mean);
                    var newMean = tr.ComputeAngle(pos);
                    if (tr.Eccentricity < 1)
                        Assert.Less(Math.Abs(MathUtil.DeltaAngle(mean, newMean)), TOLERANCE);
                    else
                        Assert.AreEqual(mean, newMean, TOLERANCE);
                }
        }

        private static void RunDynamicStaticConversion(Orbit[] trs)
        {
            foreach (var tr in trs)
                for (var i = 0; i < MASS_COUNT; i++)
                {
                    var mass = Pow(i, MASS_COUNT);
                    for (var j = 0; j < MEAN_COUNT; j++)
                    {
                        var mean = Pow(j, MEAN_COUNT);
                        var pos = tr.ComputePosition(mean);
                        var meanVel = tr.ComputeAngleSpeed(mass, 1);
                        var vel = tr.ComputeVelocity(mean, meanVel);
                        var motion = new Orbit.Motion(pos, vel, mass);
                        Assert.NotNull(motion);
                        var newTr = motion.Orbit;

                        Assert.AreEqual(tr.Apoapsis, newTr.Apoapsis, TOLERANCE, "Apoapsis");
                        Assert.AreEqual(tr.Periapsis, newTr.Periapsis, TOLERANCE, "Periapsis");
                        Assert.AreEqual(tr.Eccentricity, newTr.Eccentricity, TOLERANCE, "Eccentricity");
                        Assert.AreEqual(meanVel, motion.AngleSpeed, TOLERANCE, "MeanVelocity");
                        if (tr.Eccentricity > 0.01) Assert.Less(Math.Abs(MathUtil.DeltaAngle(tr.Argument, newTr.Argument)), TOLERANCE, "Argument");
                        if (tr.Eccentricity < 1)
                        {
                            if (tr.Eccentricity > 0.01)
                                Assert.Less(Math.Abs(MathUtil.DeltaAngle(mean, motion.Angle)), TOLERANCE, "MeanAnomaly");
                        }
                        else
                            Assert.AreEqual(mean, motion.Angle, TOLERANCE, "MeanAnomaly");
                    }
                }
        }

        private static void RunAltitudeMeanConversion(Orbit[] trs)
        {
            foreach (var tr in trs)
            {
                if (tr.Eccentricity <= 0.01) continue;
                for (var m = 0; m < MEAN_COUNT; m++)
                {
                    var mean = Pow(m, MEAN_COUNT);
                    var alt = tr.ComputeAltitude(mean);
                    var newMean = tr.ComputeAngle(alt);
                    if (tr.Eccentricity < 1)
                    {
                        var isValid = Math.Abs(MathUtil.DeltaAngle(mean, newMean)) < TOLERANCE;
                        if (!isValid) isValid = Math.Abs(MathUtil.DeltaAngle(mean, 2 * Math.PI - newMean)) < TOLERANCE;
                        Assert.IsTrue(isValid);
                    }
                    else
                        Assert.AreEqual(Math.Abs(mean), Math.Abs(newMean), TOLERANCE);
                }
            }
        }

        private static void RunAbsoluteVelocity(Orbit[] trs)
        {
            foreach (var tr in trs)
                for (var i = 0; i < MASS_COUNT; i++)
                {
                    var mass = Pow(i, MASS_COUNT);
                    for (var j = 0; j < MEAN_COUNT; j++)
                    {
                        var mean = Pow(j, MEAN_COUNT);
                        var pos = tr.ComputePosition(mean);
                        var computer = ORBIT_COMPUTER_FIELD.GetValue(tr);
                        var semiMajorAxis = (double)computer.GetType().GetProperty("SemiMajorAxis").GetValue(computer, null);

                        var expectedVel = Math.Sqrt(mass * Math.Abs(2 / pos.Magnitude - 1 / semiMajorAxis));
                        var meanVel = tr.ComputeAngleSpeed(mass, 1);
                        var actualVel = tr.ComputeVelocity(mean, meanVel).Magnitude;
                        Assert.AreEqual(expectedVel, actualVel, TOLERANCE);
                    }
                }
        }

        private static double Pow(int v, int vCount)
        {
            return MIN_VALUE * Math.Pow(10, POW_DELTA / vCount * v);
        }
    }
}