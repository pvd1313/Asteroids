/*using System;

namespace Asteroids
{
    public struct PlanarSegment
    {
        public PlanarVector PointA;
        public PlanarVector PointB;

        public static double Distance(PlanarSegment segmentA, PlanarSegment segmentB)
        {
            if (segmentA.Intersects(segmentB)) return 0;

            var distance = ComputeDistance(segmentA, segmentB.PointA);
            distance = Math.Min(distance, ComputeDistance(segmentA, segmentB.PointB));
            distance = Math.Min(distance, ComputeDistance(segmentB, segmentA.PointA));
            distance = Math.Min(distance, ComputeDistance(segmentB, segmentA.PointB));

            return distance;
        }

        private static double ComputeDistance(PlanarSegment segment, PlanarVector point)
        {
            var projected = segment.Project(point);
            return PlanarVector.Distance(point, projected);
        }

        // Original code: http://www.vcskicks.com/code-snippet/point-projection.php
        public PlanarVector Project(PlanarVector point)
        {
            var deltaX = PointB.X - PointA.X;
            if (deltaX == 0) return new PlanarVector(PointA.X, point.Y);

            var k = (PointB.Y - PointA.Y) / deltaX;
            var b = PointA.Y - k * (PointA.X);
            var sub = k * k + 1;

            var x = k * (point.Y - b) + point.X;
            var y = k * (k * point.Y  + point.X) + b;

            return new PlanarVector(x / sub, y / sub);
        }

        public PlanarSegment(PlanarVector pointA, PlanarVector pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        // Original code: http://cpp.mazurok.com/yurkin2-30/
        public bool Intersects(PlanarSegment target)
        {
            var posA0 = PointA;
            var posA1 = PointB;
            var posB0 = target.PointA;
            var posB1 = target.PointB;

            var det  = (posB0.Y - posB1.Y) * (posA1.X - posA0.X) - (posB0.X - posB1.X) * (posA1.Y - posA0.Y);
            var det1 = (posB0.Y - posA0.Y) * (posA1.X - posA0.X) - (posA1.Y - posA0.Y) * (posB0.X - posA0.X);
            var det2 = (posB0.Y - posB1.Y) * (posB0.X - posA0.X) - (posB0.X - posB1.X) * (posB0.Y - posA0.Y);

            if (det != 0)
            {
                var a = det1 / det;
                var b = det2 / det;
                return (a >= 0) && (b >= 0) && (a <= 1) && (b <= 1);
            }
            return false;
        }
    }
}*/