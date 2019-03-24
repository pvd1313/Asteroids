using NUnit.Framework;
using System;

namespace Asteroids.Test
{
    public class MathUtilTest
    {
        private const double DELTA = 1e-6;

        [Test]
        public void RepeatAngle()
        {
            Assert.AreEqual(0, MathUtil.RepeatAngle(0), DELTA, "#1");
            Assert.AreEqual(.3, MathUtil.RepeatAngle(.3), DELTA, "#2");
            Assert.AreEqual(Math.PI * 2 - .3, MathUtil.RepeatAngle(-.3), DELTA, "#3");
            Assert.AreEqual(0, MathUtil.RepeatAngle(Math.PI * 2), DELTA, "#4");
            Assert.AreEqual(0, MathUtil.RepeatAngle(Math.PI * (-2)), DELTA, "#5");
        }

        [Test]
        public void DeltaAngle()
        {
            Assert.AreEqual(.1, MathUtil.DeltaAngle(.1, .2), DELTA, "#1");
            Assert.AreEqual(-.1, MathUtil.DeltaAngle(.2, .1), DELTA, "#2");
            Assert.AreEqual(.2, MathUtil.DeltaAngle(-.1, .1), DELTA, "#3");
            Assert.AreEqual(-.2, MathUtil.DeltaAngle(.1, -.1), DELTA, "#4");
        }
    }
}