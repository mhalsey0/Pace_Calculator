using System.Drawing;
using Microsoft.AspNetCore.Components;
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
                for (int mark = 1; mark <= markerMax; mark++)
                {
                    int Marker = mark;
                    double Distance = mark;
                    TimeSpan Pace = (TimeSpan)calculatedInput.Pace;
                    TimeSpan CummulativeTime = (TimeSpan)calculatedInput.Pace * mark;
                    paceChartRows.Add(new PaceChart(Marker, Distance, Pace, CummulativeTime));                
                }
                return paceChartRows;
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
            // Initialize common variables
            List<GpxInterval> gpxIntervals = FindIntervalsFromGpxFile(gpxFile, calculatedInput.Unit);
            List<PaceChart> gradeAdjustedPaceChart = new List<PaceChart>();
            double totalDistance = Math.Round(SumDistanceFromGpxFile(gpxFile, calculatedInput.Unit), 2);
            double markerMax = Math.Ceiling(totalDistance);

            // Calculate full segments (common to both cases)
            for (int i = 0; i < markerMax; i++)
            {
                gradeAdjustedPaceChart.Add(CalculateSegment(i, gpxIntervals, calculatedInput.Pace));
            }

            // Add final partial segment if needed
            if (totalDistance % 1 != 0)
            {
                AddFinalSegment(gradeAdjustedPaceChart, calculatedInput, totalDistance);
            }

            return gradeAdjustedPaceChart;
        }

        private static PaceChart CalculateSegment(int index, List<GpxInterval> gpxIntervals, TimeSpan basePace)
        {
            int marker = index + 1;
            double distance = index + 1;
            double grade = GetAverageGradeFromListofIntervals(gpxIntervals, index, index + 1);
            TimeSpan pace = GradeAdjustedPace(basePace, CalculatePaceAdjustment(grade));
            TimeSpan cumulativeTime = pace * index;
            
            return new PaceChart(marker, distance, pace, cumulativeTime);
        }

        private static void AddFinalSegment(List<PaceChart> chart, CalculatedInput input, double totalDistance)
        {
            int finalMarker = chart.Count + 1;
            double finalDistance = Math.Round(input.Distance - Math.Floor(input.Distance), 2);
            TimeSpan finalPace = PaceCalculator(input.TotalTime, totalDistance);
            
            chart.Add(new PaceChart(finalMarker, finalDistance, finalPace, input.TotalTime));
        }

        public static List<GpxInterval> FindIntervalsFromGpxFile(GpxFile gpxFile, string unit)
        {
            List<GpxInterval> gpxIntervals = new List<GpxInterval>();
            List<double> elevation = GetElevationPoints(gpxFile);
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);

            if (coordinates.Count != elevation.Count)
            {
                System.Console.WriteLine("Count of coordinates and elevation points are not equal.");
                return gpxIntervals;
            }

            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                GpxInterval interval = CreateGpxInterval(coordinates[i], coordinates[i + 1], elevation[i], elevation[i + 1], unit);
                gpxIntervals.Add(interval);
            }

            return gpxIntervals;
        }

        private static GpxInterval CreateGpxInterval(Coordinates start, Coordinates end, double startElevation, double endElevation, string unit)
        {
            if(unit != "Miles")
            {
            double distanceBetweenPoints = CoordinatesDistanceExtensions.DistanceTo(start, end, UnitOfLength.Kilometers);
            double grade = GetGradeFromCoordinatesWithElevation(start, end, startElevation, endElevation);
            return new GpxInterval(start, end, distanceBetweenPoints, grade);
            }
            else
            {
            double distanceBetweenPoints = CoordinatesDistanceExtensions.DistanceTo(start, end, UnitOfLength.Miles);
            double grade = GetGradeFromCoordinatesWithElevation(start, end, startElevation, endElevation);
            return new GpxInterval(start, end, distanceBetweenPoints, grade);
            }
        }

        public static TimeSpan GradeAdjustedPace(TimeSpan pace, double paceAdjustment)
        {
            TimeSpan gradeAdjustedPace = TimeSpan.FromTicks((long)(pace.Ticks * paceAdjustment));
            
            return gradeAdjustedPace;
        }

        //This formula is based on the Strava gradient adjusted pace curve as discused at this link: https://pickletech.eu/blog-gap/
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
        // public static List<T> CreateNewList<T>(List<T> t)
        // {
        //     List<T> list = new List<T>();

        //     foreach (var item in t)
        //     {
        //         list.Add(item);
        //     }

        //     return list;
        // }

        public static List<GpxTrack> GetGpxTracks(GpxFile gpxFile)
        {
            List<GpxTrack> gpxTracks = new List<GpxTrack>();

            foreach (var track in gpxFile.Tracks)
            {
                gpxTracks.Add(track);
            }
            return gpxTracks;
        }
        public static List<Coordinates> CreateCoordinatesFromGpxFile(GpxFile gpxFile)
        {
            List<Coordinates> coordinates = new List<Coordinates>();

            foreach (var track in gpxFile.Tracks)
            {
                foreach(var segment in track.Segments)
                {
                    foreach (var point in segment.Points)
                    {   
                        double longitude = point.Longitude;
                        double latitude = point.Latitude;

                        coordinates.Add(new Coordinates(longitude, latitude));
                    }
                }
            }
            return coordinates;
        }
        public static int GetCountOfPoints(GpxFile gpxFile)
        {
            List<GpxTrackSegment> listOfSegments = new List<GpxTrackSegment>();


            foreach (var track in gpxFile.Tracks)
            {
                foreach(var segment in track.Segments)
                {
                    listOfSegments.Add(segment);
                }
            }
            int countOfPoints = listOfSegments.Count;
            return countOfPoints;
        }


        public static List<double> GetDistanceBetweenPointsFromGpxFile(GpxFile gpxFile)
        {
            List<double> distanceBetweenPoints = new List<double>();
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            int countOfPoints = GetCountOfPoints(gpxFile);

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

            foreach (var track in gpxFile.Tracks)
            {
                foreach (var segment in track.Segments)
                {
                    foreach(var point in segment.Points)
                    {
                        if(point.Elevation != null)
                        {
                            double elevation = Math.Round((double)point.Elevation, 3);
                            elevationPoints.Add(elevation);
                        } else {
                            elevationPoints.Add(0);
                        }
                    }

                }

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
        //This takes elements derived from the GpxFile class to provide data. It's an example of the Liskov Substitution Principle in SOLID.
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

        public static double GetAverageGradeFromListofIntervals(List<GpxInterval> intervalList, int startIndex, int endIndex)
        {
            double totalGrade = 0;
            int numberOfIndices = (endIndex - startIndex) + 1;

            for (int i = 0; i < numberOfIndices; i++)
            {
                GpxInterval interval = intervalList[startIndex + i];
                totalGrade += interval.Grade;
            }

            return totalGrade / numberOfIndices;
        }

        public static List<int> GetMarkersFromGpxFile(GpxFile gpxFile, string unit)
        {
            List<int> markers = new List<int>();
            List<GpxInterval> intervals = FindIntervalsFromGpxFile(gpxFile, unit);
            double totalDistance = 0;
            int countOfIntervals = intervals.Count;


            for (int i = 0; i < countOfIntervals; i++)
            {
                totalDistance += intervals[i].DistanceBetweenPoints;

                if (totalDistance % 1 == 0)
                {
                    markers.Add(i);
                }
            }
            return markers;
        }

        //returns distance in kilometers only for point to point maps.
        public static double GetTotalDistanceFromGpxFile(GpxFile gpxFile, string unit)
        {
            List<Coordinates> coordinates = CreateCoordinatesFromGpxFile(gpxFile);
            Coordinates start = coordinates [0];
            Coordinates end = coordinates [^1];

            if (unit != "Miles")
            {
                return CoordinatesDistanceExtensions.DistanceTo(start, end, UnitOfLength.Kilometers);
            }
            return CoordinatesDistanceExtensions.DistanceTo(start, end, UnitOfLength.Miles);
        }

        public static double SumDistanceFromGpxFile(GpxFile gpxFile, string unit)
        {
            var intervals = FindIntervalsFromGpxFile(gpxFile, unit);
            double totalDistance =  0;

            foreach (var interval in intervals)
            {
                if (!double.IsNaN(interval.DistanceBetweenPoints))
                {
                    totalDistance += interval.DistanceBetweenPoints;
                }
            }
            return totalDistance;
        }
    }
}