using System;
using NUnit.Framework;

namespace Asteroids
{
    public static class AssertUtil
    {
        public static void AreEqual(PlanarVector expected, PlanarVector actual)
        {
            if ((expected.x != actual.x) || (expected.y != actual.y))
                throw new AssertionException(string.Format("Expected: {0} But was: {1}", expected, actual));
        }

        public static void AreEqual(PlanarVector expected, PlanarVector actual, double delta)
        {
            var dX = Math.Abs(expected.x - actual.x);
            var dY = Math.Abs(expected.y - actual.y);
            if ((dX > delta) || (dY > delta))
                throw new AssertionException(string.Format("Expected: {0} But was: {1}", expected, actual));
        }

        public static void AreEqual(PlanarVector expected, PlanarVector actual, double delta, string message)
        {
            var dX = Math.Abs(expected.x - actual.x);
            var dY = Math.Abs(expected.y - actual.y);
            if ((dX > delta) || (dY > delta))
                throw new AssertionException(message);
        }
    }
}