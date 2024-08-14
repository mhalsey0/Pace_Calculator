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

        public double Grade { get; set; }
        public Coordinates Start { get; set;}
        public Coordinates End { get; set;}
        public double DistanceBetweenPoints { get; set;}
    }
}