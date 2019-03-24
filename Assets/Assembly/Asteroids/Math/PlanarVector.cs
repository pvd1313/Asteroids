using System;

namespace Asteroids
{
    [Serializable]
    public struct PlanarVector
    {
        public static readonly PlanarVector Zero  = new PlanarVector(0, 0);
        public static readonly PlanarVector XAxis = new PlanarVector(1, 0);
        public static readonly PlanarVector YAxis = new PlanarVector(0, 1);
       
        public double X;
        public double Y;

        public static double Cross(PlanarVector a, PlanarVector b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static double Dot(PlanarVector a, PlanarVector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static PlanarVector Cross(PlanarVector vec)
        {
            return new PlanarVector(-vec.Y, vec.X);
        }

        public PlanarVector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y); } 
        }

        public double SqrMagnitude
        {
            get { return X * X + Y * Y; }
        }

        public static double Distance(PlanarVector pointA, PlanarVector pointB)
        {
            var dX = pointA.X - pointB.X;
            var dY = pointA.Y - pointB.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public static double SqrDistance(PlanarVector pointA, PlanarVector pointB)
        {
            var dX = pointA.X - pointB.X;
            var dY = pointA.Y - pointB.Y;
            return dX * dX + dY * dY;
        }

        public PlanarVector Normalized
        {
            get { return this / Magnitude; }
        }

        public void Swap()
        {
            var t = X;
            X = Y;
            Y = t;
        }

        public static PlanarVector operator /(PlanarVector vector, double value)
        {
            return new PlanarVector(vector.X / value, vector.Y / value);
        }

        public static PlanarVector operator *(PlanarVector vector, double value)
        {
            return new PlanarVector(vector.X * value, vector.Y * value);
        }

        public static PlanarVector operator +(PlanarVector a, PlanarVector b)
        {
            return new PlanarVector(a.X + b.X, a.Y + b.Y);
        }

        public static PlanarVector operator -(PlanarVector a, PlanarVector b)
        {
            return new PlanarVector(a.X - b.X, a.Y - b.Y);
        }

        public static PlanarVector operator -(PlanarVector a)
        {
            return new PlanarVector(-a.X, -a.Y);
        }

        public override string ToString()
		{
			return string.Format ("({0:N2}, {1:N2})", X, Y);
		}
    }
}