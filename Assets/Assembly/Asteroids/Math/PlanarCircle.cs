using System;

namespace Asteroids
{
    public struct PlanarCircle
    {
        public PlanarVector Position;
        public double Radius;
        public double Perimeter { get { return Math.PI * Radius * Radius; } }

        public PlanarCircle(PlanarVector position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        public double ComputeBorderDistance(PlanarCircle target)
        {
            var big = target;
            var small = this;
            if (big.Radius < small.Radius) Util.Swap(ref big, ref small);
            var m = PlanarVector.Distance(small.Position, big.Position);

            if (m > big.Radius + small.Radius)
                return m - big.Radius - small.Radius;
            if (m < big.Radius - small.Radius)
                return big.Radius - m - small.Radius;
            return 0;
        }
    }
}