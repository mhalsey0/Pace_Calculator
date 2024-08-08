namespace Pace_Calculator
{
    public class GpxInterval
    {
        public GpxInterval(Coordinates start, Coordinates end, double distanceBetweenPoints, double grade)
        {
            Start = start;
            End = end;
            DistanceBetweenPoints = distanceBetweenPoints;
            Grade = grade;
        }

        public List<Coordinates> StartCoordinates { get; set; }
        public List<Coordinates> EndCoordinates { get; set;}
        public double Distance { get; set; }
        public double Grade { get; set; }
        public Coordinates Start { get; }
        public Coordinates End { get; }
        public double DistanceBetweenPoints { get; }
    }
}