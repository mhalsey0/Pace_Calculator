namespace Pace_Calculator
{
    //Holds information required to populate a pace chart. This is an example of single use principle in SOLID.
    public class PaceChart
    {
        public int Marker { get; set; }
        public double Distance { get; set; }
        public TimeSpan Pace { get; set; }
        public TimeSpan CummulativeTime { get; set; }

        public PaceChart(int marker, double distance, TimeSpan pace, TimeSpan cummulativeTime)
        {
            Marker = marker;
            Distance = distance;
            Pace = pace;
            CummulativeTime = cummulativeTime;
        }
    }
}