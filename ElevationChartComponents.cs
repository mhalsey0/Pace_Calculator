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


        public List<Coordinates> CreateCoordinatesFromGpxFile(GpxFile gpxFile)
        {
            List<Coordinates> coordinates = new List<Coordinates>();

            foreach (var point in gpxFile.Waypoints)
            {
                double longitude = point.Longitude;
                double latitude = point.Latitude;
                coordinates.Add(new Coordinates(longitude, latitude));
            }
            return coordinates;
        }

        public List<double> GetDistanceBetweenPointsFromGpxFile(GpxFile gpxFile, CalculatedInput calculatedInput)
        {
            List<double> distanceBetweenPoints = new List<double>();
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            int countOfPoints = gpxFile.Waypoints.Count;
            UnitOfLength unitOfLength = calculatedInput.Unit;

            for (int i = 0; i < countOfPoints;)
            {
                Coordinates start = coordinates[i];
                Coordinates end = coordinates[i + 1];
                distanceBetweenPoints.Add(CoordinatesDistanceExtensions.DistanceTo( start, end, unitOfLength));
            }

            return distanceBetweenPoints;
        }

    }
}