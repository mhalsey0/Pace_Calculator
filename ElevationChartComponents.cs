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

        public List<double> GetElevationPoints(GpxFile gpxFile)
        {
            List<double> elevationPoints = new List<double>();

            foreach (var point in gpxFile.Waypoints)
            {
                double elevation = (double)point.Elevation;
                elevationPoints.Add(elevation);
            }
            return elevationPoints;
        }

        public List<double> GetGradeBetweenPoints(GpxFile gpxFile, CalculatedInput calculatedInput)
        {
            List<double> grade = new List<double>();
            List<double> distanceBetweenPoints = GetDistanceBetweenPointsFromGpxFile(gpxFile, calculatedInput);
            List<double> elevationPoints = GetElevationPoints(gpxFile);
            elevationPoints.RemoveAt(elevationPoints.Count - 1);

            for (int i = 0; i < elevationPoints.Count;)
            {
                if(distanceBetweenPoints[i] == 0)
                {
                    grade.Add(double.NaN);
                }
                grade.Add(elevationPoints[i] / distanceBetweenPoints [i]);
            }
            return grade;
        }

        public List<int> GetMarkersFromGpxFile(GpxFile gpxFile, CalculatedInput calculatedInput)
        {
            List<int> markers = new List<int>();
            UnitOfLength unitOfLength = calculatedInput.Unit;
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            Coordinates start = coordinates[0];
            Coordinates end = coordinates[^1];
            double totalDistance = CoordinatesDistanceExtensions.DistanceTo(start, end, calculatedInput.Unit);

            for (int mark = 0; mark < totalDistance+1;)
            {
                markers.Add(mark);
            }

            return markers;


        }

    }
}