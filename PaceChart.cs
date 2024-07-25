namespace Pace_Calculator
{
    public class PaceChart
    {
        public PaceChart(int marker, double distance, TimeSpan pace, TimeSpan cummulativeTime)
        {
            Marker = marker;
            Distance = distance;
            Pace = pace;
            CummulativeTime = cummulativeTime;
        }

        public int Marker { get; set; }
        public double Distance { get; set; }
        public TimeSpan Pace { get; set; }
        public TimeSpan CummulativeTime { get; set; }
    }
}
