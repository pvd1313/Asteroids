using NUnit.Framework;

namespace Asteroids.Test
{
    public class PlanarCircleTest
    {
        private const double DELTA = 1e-3;

        [Test]
        public void ComputeBorderDistance()
        {
            PlanarCircle circle0, circle1;

            circle0 = new PlanarCircle(new PlanarVector(0, 0), 3);
            circle1 = new PlanarCircle(new PlanarVector(5, 0), 1);
            Assert.AreEqual(1, circle0.ComputeBorderDistance(circle1), DELTA, "#1A");
            Assert.AreEqual(1, circle1.ComputeBorderDistance(circle0), DELTA, "#1B");

            circle0 = new PlanarCircle(new PlanarVector(3, 07), 2);
            circle1 = new PlanarCircle(new PlanarVector(3, 12), 1);
            Assert.AreEqual(2, circle0.ComputeBorderDistance(circle1), DELTA, "#2A");
            Assert.AreEqual(2, circle1.ComputeBorderDistance(circle0), DELTA, "#2B");

            circle0 = new PlanarCircle(new PlanarVector(0, 0), 10);
            circle1 = new PlanarCircle(new PlanarVector(5, 0), 3);
            Assert.AreEqual(2, circle0.ComputeBorderDistance(circle1), DELTA, "#3A");
            Assert.AreEqual(2, circle1.ComputeBorderDistance(circle0), DELTA, "#3B");

            circle0 = new PlanarCircle(new PlanarVector(0, 0), 6);
            circle1 = new PlanarCircle(new PlanarVector(3, 0), 4);
            Assert.AreEqual(0, circle0.ComputeBorderDistance(circle1), DELTA, "#4A");
            Assert.AreEqual(0, circle1.ComputeBorderDistance(circle0), DELTA, "#4B");
        }
    }
}
