namespace Asteroids
{
    [System.Serializable]
    public struct MinMax
    {
        public double Min;
        public double Max;

        public MinMax(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public bool IsInside(double value)
        {
            return value >= Min && value <= Max;
        }
    }
}