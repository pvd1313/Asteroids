/*using System;
using NUnit.Framework;

namespace Asteroids.Gravity.Test
{
    public class PlanarSegmentTest
    {
        [Test]
        public void IntersectionTest()
        {
            var segmentA = new PlanarSegment(new PlanarVector(-3, 1), new PlanarVector(-1, 1));
            var segmentB = new PlanarSegment(new PlanarVector(-2, 3), new PlanarVector(0, 3));
            var distance = PlanarSegment.Distance(segmentA, segmentB);
            Assert.AreEqual(2, distance, "#1A");
            distance = PlanarSegment.Distance(segmentB, segmentA);
            Assert.AreEqual(2, distance, "#1B");

            segmentA = new PlanarSegment(new PlanarVector(-2, 1), new PlanarVector(-2, -1));
            segmentB = new PlanarSegment(new PlanarVector(1, 2), new PlanarVector(1, -2));
            distance = PlanarSegment.Distance(segmentA, segmentB);
            Assert.AreEqual(3, distance, "#2A");
            distance = PlanarSegment.Distance(segmentB, segmentA);
            Assert.AreEqual(3, distance, "#2B");

            segmentA = new PlanarSegment(new PlanarVector(1, 1), new PlanarVector(-1, -1));
            segmentB = new PlanarSegment(new PlanarVector(-1, 1), new PlanarVector(1, -1));
            distance = PlanarSegment.Distance(segmentA, segmentB);
            Assert.AreEqual(0, distance, "#3A");
            distance = PlanarSegment.Distance(segmentB, segmentA);
            Assert.AreEqual(0, distance, "#3B");
        }

        [Test]
        public void PointProjectionTest()
        {
            var pointA = new PlanarVector(-1, 1);
            var pointB = new PlanarVector(1, 1);
            var pointC = new PlanarVector(0, 2);
            var segment = new PlanarSegment(pointA, pointB);
            Assert.AreEqual(new PlanarVector(0, 1), segment.Project(pointC), "#1");

            pointA = new PlanarVector(0, 2);
            pointB = new PlanarVector(2, 0);
            pointC = new PlanarVector(2, 2);
            segment = new PlanarSegment(pointA, pointB);
            Assert.AreEqual(new PlanarVector(1, 1), segment.Project(pointC), "#2");

            pointA = new PlanarVector(-1, 1);
            pointB = new PlanarVector(-1, -1);
            pointC = new PlanarVector(1, 1);
            segment = new PlanarSegment(pointA, pointB);
            Assert.AreEqual(new PlanarVector(-1, 1), segment.Project(pointC), "#3");
        }
    }
}*/