using RolandK.Formats.Gpx;


namespace Pace_Calculator
{
    public class Calculators
    {
        public static CalculatedInput Calculate(UserInput userInput)
        {
            if (userInput.Pace <= TimeSpan.Zero)
            {
                var pace = PaceCalculator(userInput.TotalTime, userInput.Distance);
                return new CalculatedInput() {  Distance = userInput.Distance, 
                                                TotalTime = userInput.TotalTime, 
                                                Pace = pace,
                                                Unit = userInput.Unit};
            }
            if (userInput.Distance <= 0)
            {
                var distance = DistanceCalculator(userInput.TotalTime, userInput.Pace);
                return new CalculatedInput() {  Distance = distance, 
                                                TotalTime = userInput.TotalTime, 
                                                Pace = userInput.Pace,
                                                Unit = userInput.Unit};
            }
            if (userInput.TotalTime <= TimeSpan.Zero)
            {
                var totalTime = TotalTimeCalculator(userInput.Pace, userInput.Distance);
                return new CalculatedInput() {  Distance = userInput.Distance, 
                                                TotalTime = totalTime, 
                                                Pace = userInput.Pace,
                                                Unit = userInput.Unit};
            }
            return CalculatedInput.FromUserInput(userInput);
        }
        
        //Will need to account for units in formatting for display
        public static TimeSpan PaceCalculator(TimeSpan totalTime, double distance)
        {
            if (distance == 0)
            {
                return TimeSpan.Zero;
            }
            TimeSpan pace = totalTime / distance;
            return pace; // pace is equal to total time divided by total distance
        }

        public static double DistanceCalculator(TimeSpan totalTime, TimeSpan pace)
        {        
            if (pace == TimeSpan.Zero)
            {
                return 0;
            }
            double distance = totalTime.TotalSeconds / pace.TotalSeconds; 
            return distance; //distance is equal to total time divided  by pace
        }

        public static TimeSpan TotalTimeCalculator(TimeSpan pace, double distance)
        {        
            TimeSpan totalTime = TimeSpan.FromSeconds(pace.TotalSeconds * distance);
            return totalTime; //total time is equal to pace multiplied by distance
        }

        public static List<PaceChart> CalculatePaceChart(CalculatedInput calculatedInput)
        {
            List<PaceChart> paceChartRows = new List<PaceChart>();

            int markerMax = (int)Math.Ceiling((double)calculatedInput.Distance);

            if (calculatedInput.Distance - Math.Floor((double)calculatedInput.Distance) == 0)
            {
                for (int mark = 0; mark <= markerMax; mark++)
                {
                    int Marker = mark;
                    double Distance = mark;
                    TimeSpan Pace = (TimeSpan)calculatedInput.Pace;
                    TimeSpan CummulativeTime = (TimeSpan)calculatedInput.Pace * mark;
                    paceChartRows.Add(new PaceChart(Marker, Distance, Pace, CummulativeTime));                
                }
            }
            else
            {
                for(int mark = 1; mark < markerMax; mark++)
                {
                    int Marker = mark;
                    double Distance = mark;
                    TimeSpan Pace = (TimeSpan)calculatedInput.Pace;
                    TimeSpan CummulativeTime = (TimeSpan)calculatedInput.Pace * mark;
                    paceChartRows.Add(new PaceChart(Marker, Distance, Pace, CummulativeTime));                
                }
                int FinalMarker = paceChartRows.Count;
                double FinalDistance = Math.Round((double)calculatedInput.Distance - Math.Floor((double)calculatedInput.Distance),2);
                TimeSpan FinalPace = (TimeSpan) calculatedInput.Pace;
                TimeSpan FinalCummulativeTime = (TimeSpan)calculatedInput.TotalTime;
                paceChartRows.Add(new PaceChart(FinalMarker, FinalDistance, FinalPace, FinalCummulativeTime));
                return paceChartRows;
            }
        }

        public static List<PaceChart> CalculateGradeAdjustedPaceChart(GpxFile gpxFile, CalculatedInput calculatedInput)
        {
            List<GpxInterval> gpxIntervals = FindIntervalsFromGpxFile(gpxFile);
            List<PaceChart> gradeAdjustedPaceChart = new List<PaceChart>();
            double totalDistance = Math.Round(GetTotalDistanceFromGpxFile(gpxFile, calculatedInput.Unit),2);
            double markerMax = Math.Ceiling(totalDistance);

            if( totalDistance - Math.Floor(totalDistance) == 0)
            {
                for (int mark = 0; mark <= markerMax; mark++)
                {
                    int Marker = mark+1;
                    double Distance = mark+1;
                    double grade = gpxIntervals[mark].Grade;
                    TimeSpan Pace = GradeAdjustedPace(calculatedInput.Pace,CalculatePaceAdjustment(grade));
                    TimeSpan CummulativeTime = (TimeSpan)Pace * mark;
                    gradeAdjustedPaceChart.Add(new PaceChart(Marker, Distance, Pace, CummulativeTime));                
                }
                return gradeAdjustedPaceChart;
            }else
            {
                for (int mark = 0; mark <= markerMax; mark++)
                {
                    int Marker = mark+1;
                    double Distance = mark+1;
                    double grade = gpxIntervals[mark].Grade;
                    TimeSpan Pace = GradeAdjustedPace(calculatedInput.Pace,CalculatePaceAdjustment(grade));
                    TimeSpan CummulativeTime = Pace * mark;
                    gradeAdjustedPaceChart.Add(new PaceChart(Marker, Distance, Pace, CummulativeTime));                
                }
                int FinalMarker = gradeAdjustedPaceChart.Count;
                double FinalDistance = Math.Round((double)calculatedInput.Distance - Math.Floor((double)calculatedInput.Distance),2);
                TimeSpan FinalPace = PaceCalculator(calculatedInput.TotalTime, totalDistance);
                TimeSpan FinalCummulativeTime = (TimeSpan)calculatedInput.TotalTime;
                gradeAdjustedPaceChart.Add(new PaceChart(FinalMarker, FinalDistance, FinalPace, FinalCummulativeTime));
                return gradeAdjustedPaceChart;                 
            }
        }
        public static List<GpxInterval> FindIntervalsFromGpxFile(GpxFile gpxFile)
        {
            List<GpxInterval> gpxIntervals = new List<GpxInterval>();
            List<double> elevation = GetElevationPoints(gpxFile);
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            int countOfPoints = gpxFile.Waypoints.Count;

            if(countOfPoints != coordinates.Count || countOfPoints != elevation.Count)
            {
                System.Console.WriteLine("waypoints does not equal the count of coordinates or count of elevation points");
                return null;
            }

            for (int i = 0; i < countOfPoints - 1; i++)
            {
                Coordinates start = coordinates[i];
                Coordinates end = coordinates[i + 1];
                double startElevation = elevation[i];
                double endElevation = elevation[i + 1];
                double distanceBetweenPoints = CoordinatesDistanceExtensions.DistanceTo(start, end);
                double grade = GetGradeFromCoordinatesWithElevation(start, end, startElevation, endElevation);
                gpxIntervals.Add(new GpxInterval(start, end, distanceBetweenPoints, grade));
            }
            return gpxIntervals;
        }

        public static TimeSpan GradeAdjustedPace(TimeSpan pace, double paceAdjustment)
        {
            TimeSpan gradeAdjustedPace = TimeSpan.FromTicks((long)(pace.Ticks * paceAdjustment));
            
            return gradeAdjustedPace;
        }

        public static double CalculatePaceAdjustment(double grade)
        {
            // Constants for polynomial equation for PaceAdjustment
            double a4 = -533.33;
            double a3 = 26.67;
            double a2 = 25.33;
            double a1 = 2.73;
            double a0 = 1.0;

            // Variable for polynomial equation
            double x = Math.Max(-0.1, Math.Min(grade, 0.1));

            double PaceAdjustment = a4 * Math.Pow(x, 4) + a3 * Math.Pow(x, 3) + a2 * Math.Pow(x, 2) + a1 * x + a0;
            return PaceAdjustment;
        }

        public static List<Coordinates> CreateCoordinatesFromGpxFile(GpxFile gpxFile)
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

        public static List<double> GetDistanceBetweenPointsFromGpxFile(GpxFile gpxFile)
        {
            List<double> distanceBetweenPoints = new List<double>();
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            int countOfPoints = gpxFile.Waypoints.Count;

            for (int i = 0; i < countOfPoints - 1; i++)
            {
                Coordinates start = coordinates[i];
                Coordinates end = coordinates[i + 1];
                distanceBetweenPoints.Add(CoordinatesDistanceExtensions.DistanceTo( start, end));
            }

            return distanceBetweenPoints;
        }

        public static List<double> GetElevationPoints(GpxFile gpxFile)
        {
            List<double> elevationPoints = new List<double>();

            foreach (var point in gpxFile.Waypoints)
            {
                double elevation = point.Elevation ?? 0;
                elevationPoints.Add(elevation);
            }
            return elevationPoints;
        }

        public static List<double> GetGradeBetweenPoints(GpxFile gpxFile)
        {
            List<double> grade = new List<double>();
            List<double> distanceBetweenPoints = GetDistanceBetweenPointsFromGpxFile(gpxFile);
            List<double> elevationPoints = GetElevationPoints(gpxFile);
            elevationPoints.RemoveAt(elevationPoints.Count - 1);

            for (int i = 0; i < elevationPoints.Count; i++)
            {
                if(distanceBetweenPoints[i] == 0)
                {
                    grade.Add(double.NaN);
                }
                grade.Add((elevationPoints[i] - elevationPoints[i +1])/ (distanceBetweenPoints [i] * 1000));
            }
            return grade;
        }

        public static double GetGradeFromCoordinatesWithElevation(Coordinates start, Coordinates end, double startElevation, double endElevation)
        {
            double distance = CoordinatesDistanceExtensions.DistanceTo(start, end);
            double elevationChange = startElevation - endElevation;
            if ( distance <= 0)
            {
                return 0;
            }
            double grade = elevationChange / (distance * 1000);
            
            return grade;
        }



        public static List<int> GetMarkersFromGpxFile(GpxFile gpxFile, UnitOfLength unitOfLength)
        {
            List<int> markers = new List<int>();
            UnitOfLength unit = unitOfLength;
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            Coordinates start = coordinates[0];
            Coordinates end = coordinates[^1];
            double totalDistance = CoordinatesDistanceExtensions.DistanceTo(start, end, unit);

            for (int mark = 0; mark <= totalDistance; mark++)
            {
                markers.Add(mark);
            }

            return markers;
        }
        public static double GetTotalDistanceFromGpxFile(GpxFile gpxFile, UnitOfLength unitOfLength)
        {
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            UnitOfLength unit = unitOfLength;
            Coordinates start = coordinates [0];
            Coordinates end = coordinates [^1];

            return CoordinatesDistanceExtensions.DistanceTo(start, end, unit);
        }
    }
}