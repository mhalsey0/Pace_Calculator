using RolandK.Formats.Gpx;



namespace Pace_Calculator
{
    public class ElevationChartComponents
    {
        public List<Coordinates> Coordinates { get; set; }
        public List<double> Elevation { get; set; }
        public List<double> Distance { get; set; }
        public List<double> Grade { get; set; }
        public List<int> Marker { get; set; }
        public CalculatedInput CalculatedInput { get; set; }

    }
}