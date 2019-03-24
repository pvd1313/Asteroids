using System;

namespace Asteroids
{
    public struct PlanarRotation
    {
        public double Angle { get { return Math.Atan2(_sin, _cos); } }

        private double _sin;
        private double _cos;

        public static PlanarVector Rotate(double xComponent, double angle)
        {
            return new PlanarVector(xComponent * Math.Cos(angle), xComponent * Math.Sin(angle));
        }

        public static PlanarVector Rotate(PlanarVector vector, double angle)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);
            return new PlanarVector(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
        }

        public PlanarRotation(double angle)
        {
            _sin = Math.Sin(angle);
            _cos = Math.Cos(angle);
        }

        public PlanarVector Rotate(PlanarVector source)
        {
            return new PlanarVector(source.X * _cos - source.Y * _sin, source.X * _sin + source.Y * _cos);
        }

        public PlanarVector Rotate(double xComponent)
        {
            return new PlanarVector(xComponent * _cos, xComponent * _sin);
        }

        public PlanarVector InvertedRotate(PlanarVector source)
        {
            return new PlanarVector(source.X * _cos + source.Y * _sin, source.Y * _cos - source.X * _sin);
        }

        public PlanarVector InvertedRotate(double xComponent)
        {
            return new PlanarVector(xComponent * _cos, -xComponent * _sin);
        }

        public override string ToString()
        {
            return string.Format("a:{0:N0} s:{1:N2} c:{2:N2}", Angle * MathUtil.Rad2Deg, _sin, _cos);
        }
    }
}