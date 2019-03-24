using System;

namespace Asteroids
{
    public static class MathUtil
    {
        public const double Deg2Rad = 1.0 / 180 * Math.PI;
        public const double Rad2Deg = 1.0 / Math.PI * 180;

        public static double Acosh(double cosh)
        {
            return Math.Log(cosh + Math.Sqrt(cosh + 1) * Math.Sqrt(cosh - 1));
        }

        public static double Atanh(double tanh)
        {
            return Math.Log((1 + tanh) / (1 - tanh)) / 2;
        }

        public static double Repeat(double t, double length)
        {
            t %= length;
            if (t < 0) return length + t;
            return t;
        }

        public static double RepeatAngle(double angle)
        {
            if (angle >= 2 * Math.PI) return angle % (2 * Math.PI);
            if (angle < 0) return 2 * Math.PI + angle % (2 * Math.PI);
            return angle;
        }

        public static double DeltaAngle(double current, double target)
        {
            current = RepeatAngle(current);
            target = RepeatAngle(target);
            double clockwise, counter;
            if (target > current)
            {
                clockwise = target - current;
                counter = current + (2 * Math.PI - target);      
            }
            else
            {
                counter = current - target;
                clockwise = target + (2 * Math.PI - current);
            }
            return clockwise < counter ? clockwise : -counter;
        }

        public static double DeltaAngle(double current, double target, double direction)
        {
            current = RepeatAngle(current);
            target = RepeatAngle(target);
            if (direction >= 0)
            {
                if (target >= current) return target - current;
                return target + 2 * Math.PI - current;
            }
            else
            {
                if (current >= target) return -(current - target);
                return -(current + 2 * Math.PI - target);
            }
        }

        public static double PositiveDeltaAngle(double current, double target)
        {
            current = RepeatAngle(current);
            target = RepeatAngle(target);
            if (target >= current) return target - current;
            return target + 2 * Math.PI - current;
        }

        public static double NegativeDeltaAngle(double current, double target)
        {
            current = RepeatAngle(current);
            target = RepeatAngle(target);
            if (current >= target) return -(current - target);
            return -(current + 2 * Math.PI - target);
        }
    }
}
