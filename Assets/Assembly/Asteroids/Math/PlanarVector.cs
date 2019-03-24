using System;

namespace Asteroids
{
    [Serializable]
    public struct PlanarVector
    {
        public static readonly PlanarVector zero  = new PlanarVector(0, 0);
        public static readonly PlanarVector xAxis = new PlanarVector(1, 0);
        public static readonly PlanarVector yAxis = new PlanarVector(0, 1);
       
        public double x;
        public double y;

        public static double Cross(PlanarVector a, PlanarVector b)
        {
            return a.x * b.y - a.y * b.x;
        }

        public static double Dot(PlanarVector a, PlanarVector b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static PlanarVector Cross(PlanarVector vec)
        {
            return new PlanarVector(-vec.y, vec.x);
        }

        public PlanarVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double Magnitude
        {
            get { return Math.Sqrt(x * x + y * y); } 
        }

        public double SqrMagnitude
        {
            get { return x * x + y * y; }
        }

        public static double Distance(PlanarVector pointA, PlanarVector pointB)
        {
            var dX = pointA.x - pointB.x;
            var dY = pointA.y - pointB.y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public static double SqrDistance(PlanarVector pointA, PlanarVector pointB)
        {
            var dX = pointA.x - pointB.x;
            var dY = pointA.y - pointB.y;
            return dX * dX + dY * dY;
        }

        public PlanarVector Normalized
        {
            get { return this / Magnitude; }
        }

        public void Swap()
        {
            var t = x;
            x = y;
            y = t;
        }

        public static PlanarVector operator /(PlanarVector vector, double value)
        {
            return new PlanarVector(vector.x / value, vector.y / value);
        }

        public static PlanarVector operator *(PlanarVector vector, double value)
        {
            return new PlanarVector(vector.x * value, vector.y * value);
        }

        public static PlanarVector operator +(PlanarVector a, PlanarVector b)
        {
            return new PlanarVector(a.x + b.x, a.y + b.y);
        }

        public static PlanarVector operator -(PlanarVector a, PlanarVector b)
        {
            return new PlanarVector(a.x - b.x, a.y - b.y);
        }

        public static PlanarVector operator -(PlanarVector a)
        {
            return new PlanarVector(-a.x, -a.y);
        }

        public override string ToString()
		{
			return string.Format ("({0:N2}, {1:N2})", x, y);
		}
    }
}