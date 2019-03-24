namespace Asteroids
{
    [System.Serializable]
    public struct MinMax
    {
        public double min;
        public double max;

        public MinMax(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public bool IsInside(double value)
        {
            return value >= min && value <= max;
        }
    }
}