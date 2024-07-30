namespace Pace_Calculator
{
    public class GpxFile
    {
        public string FileName { get; set; }
        public double TotalDistance { get; set; }
        public double TotalElevation { get; set; }
        public List<TrackPoint> TrackPoints { get; set; }
    }
}