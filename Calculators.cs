using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pace_Calculator
{
    public class Calculators
    {
        public static void Calculate(UserInput userInput)
        {
            PropertyInfo[] userInputProperties = userInput.GetType().GetProperties();
            bool needsCalculation = true;

            foreach (PropertyInfo property in userInputProperties)
            {
                var propertyValue = property.GetValue(userInput);
                
                if (propertyValue == null)
                {
                    continue;
                }
                
                if (propertyValue.Equals(0))
                {
                    if (property.PropertyType == typeof(TimeSpan) && property.Name == "Pace") 
                    {
                        userInput.Pace = PaceCalculator(userInput.TotalTime, userInput.Distance);
                        needsCalculation = false;
                    }
                    if (property.PropertyType == typeof(double) && property.Name == "Distance") 
                    {
                        userInput.Distance = DistanceCalculator(userInput.TotalTime, userInput.Pace);
                        needsCalculation = false;
                    }
                    if (property.PropertyType == typeof(TimeSpan) && property.Name == "TotalTime") 
                    {
                        userInput.TotalTime = TotalTimeCalculator(userInput.Pace, userInput.Distance);
                        needsCalculation = false;
                    }
                }
            }

            if (needsCalculation)
            {
                toCalculate(userInput);
            }
        }

        public static void toCalculate(UserInput userInput)
        {
            TimeSpan pace = PaceCalculator(userInput.TotalTime, userInput.Distance);
            double distance = DistanceCalculator(userInput.TotalTime, userInput.Pace);
            TimeSpan totalTime = TotalTimeCalculator(userInput.Pace, userInput.Distance);

            if (userInput.Pace != pace && userInput.Distance != distance)
            {
                userInput.Pace = pace;
            }
            if (userInput.Distance != distance && userInput.TotalTime != totalTime)
            {
                userInput.TotalTime = totalTime;
            }
            if (userInput.TotalTime != totalTime && userInput.Pace != pace)
            {
                userInput.TotalTime = totalTime;
            }
        }

        public static TimeSpan PaceCalculator(TimeSpan totalTime, double distance)
        {
            if (distance == 0) return TimeSpan.Zero;
            TimeSpan pace = totalTime / distance;
            return pace; // pace is equal to total time divided by total distance
        }

        public static double DistanceCalculator(TimeSpan totalTime, TimeSpan pace)
        {
            if (pace == TimeSpan.Zero) return 0;
            double distance = totalTime.TotalMinutes / pace.TotalMinutes;
            return distance; // distance is equal to total time divided by pace
        }

        public static TimeSpan TotalTimeCalculator(TimeSpan pace, double distance)
        {
            TimeSpan totalTime = TimeSpan.FromMinutes(pace.TotalMinutes * distance);
            return totalTime; // total time is equal to pace multiplied by distance
        }

        public static List<PaceChart> CalculatePaceChart(UserInput userInput)
        {
            List<PaceChart> paceChartRows = new List<PaceChart>();

            int markerMax = (int)Math.Ceiling(userInput.Distance);

            if (userInput.Distance - Math.Floor(userInput.Distance) == 0)
            {
                for (int mark = 0; mark <= markerMax; mark++)
                {
                    int Marker = mark;
                    double Distance = mark;
                    TimeSpan Pace = userInput.Pace;
                    TimeSpan CumulativeTime = userInput.Pace * mark;
                    paceChartRows.Add(new PaceChart(Marker, Distance, Pace, CumulativeTime));
                }
            }
            else
            {
                for (int mark = 0; mark <= markerMax - 1; mark++)
                {
                    int Marker = mark;
                    double Distance = mark;
                    TimeSpan Pace = userInput.Pace;
                    TimeSpan CumulativeTime = userInput.Pace * mark;
                    paceChartRows.Add(new PaceChart(Marker, Distance, Pace, CumulativeTime));
                }
                int FinalMarker = paceChartRows.Count;
                double FinalDistance = userInput.Distance - Math.Floor(userInput.Distance);
                TimeSpan FinalPace = userInput.Pace;
                TimeSpan FinalCumulativeTime = userInput.TotalTime;
                paceChartRows.Add(new PaceChart(FinalMarker, FinalDistance, FinalPace, FinalCumulativeTime));
            }

            return paceChartRows;
        }

        public static TimeSpan GradeAdjustedPace(TimeSpan pace, double paceAdjustment)
        {
            TimeSpan gradeAdjustedPace = TimeSpan.FromTicks((long)(pace.Ticks * paceAdjustment)); // Corrected multiplication logic
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
    }
}
