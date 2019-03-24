using System;
using NUnit.Framework;

namespace Asteroids
{
    public static class AssertUtil
    {
        public static void AreEqual(PlanarVector expected, PlanarVector actual)
        {
            if ((expected.X != actual.X) || (expected.Y != actual.Y))
                throw new AssertionException(string.Format("Expected: {0} But was: {1}", expected, actual));
        }

        public static void AreEqual(PlanarVector expected, PlanarVector actual, double delta)
        {
            var dX = Math.Abs(expected.X - actual.X);
            var dY = Math.Abs(expected.Y - actual.Y);
            if ((dX > delta) || (dY > delta))
                throw new AssertionException(string.Format("Expected: {0} But was: {1}", expected, actual));
        }

        public static void AreEqual(PlanarVector expected, PlanarVector actual, double delta, string message)
        {
            var dX = Math.Abs(expected.X - actual.X);
            var dY = Math.Abs(expected.Y - actual.Y);
            if ((dX > delta) || (dY > delta))
                throw new AssertionException(message);
        }
    }
}