namespace Pace_Calculator
{
    public class TrackPoint
    {
        public int Marker { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public double DistanceFromPreviousMarker { get; set; }
        public double GradeFromPreviousMarker   { get; set; }
    }
}