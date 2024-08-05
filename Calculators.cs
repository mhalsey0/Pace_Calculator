using RolandK.Formats.Gpx;

namespace Pace_Calculator
{
    public class Calculators
    {
        public static void Calculate(UserInput userInput)
        {
            TimeSpan pace = PaceCalculator((TimeSpan)userInput.TotalTime, (double)userInput.Distance);
            Double distance = DistanceCalculator((TimeSpan)userInput.TotalTime, (TimeSpan)userInput.Pace);
            TimeSpan totalTime = TotalTimeCalculator((TimeSpan)userInput.Pace, (double)userInput.Distance);

            if (userInput.Pace != pace && userInput.Distance != distance)
            {
                userInput.Pace = pace;
                return;
            }
            if (userInput.Distance != distance && userInput.TotalTime != totalTime)
            {
                userInput.TotalTime = totalTime;
                return;
            }
            if (userInput.TotalTime != totalTime && userInput.Pace != pace)
            {
                userInput.Distance = distance;
                return;
            }
            return;
        }
        
        //Will need to account for units in formatting for display
        public static TimeSpan PaceCalculator(TimeSpan totalTime, double distance)
        {
            if (distance == 0)
            {
                return TimeSpan.Zero;
            }
            TimeSpan pace = totalTime / distance;
            return pace; //pace is equal to total time divided by total distance
        }
        public static double DistanceCalculator(TimeSpan totalTime, TimeSpan pace)
        {        
            if (pace == TimeSpan.Zero)
            {
                return 0;
            }
            double distance = totalTime / pace; 
            return distance; //distance is equal to total time divided  by pace
        }
        public static TimeSpan TotalTimeCalculator(TimeSpan pace, double distance)
        {        
            TimeSpan totalTime = pace * distance;
            return totalTime; //total time is equal to pace multiplied by distance
        }

        public static List<PaceChart> CalculatePaceChart(UserInput userInput)
        {
            List<PaceChart> paceChartRows = new List<PaceChart>();

            int markerMax = (int)Math.Ceiling((double)userInput.Distance);

            if (userInput.Distance - Math.Floor((double)userInput.Distance) == 0)
            {
                for(int mark = 0; mark <= markerMax; mark++)
                {
                    int Marker = mark;
                    double Distance = mark;
                    TimeSpan Pace = (TimeSpan)userInput.Pace;
                    TimeSpan CummulativeTime = (TimeSpan)userInput.Pace * mark;
                    paceChartRows.Add(new PaceChart(Marker, Distance, Pace, CummulativeTime));                
                }
                return paceChartRows;
            } else
            {
                for(int mark = 0; mark <= markerMax-1; mark++)
                {
                    int Marker = mark;
                    double Distance = mark;
                    TimeSpan Pace = (TimeSpan)userInput.Pace;
                    TimeSpan CummulativeTime = (TimeSpan)userInput.Pace * mark;
                    paceChartRows.Add(new PaceChart(Marker, Distance, Pace, CummulativeTime));                
                }
                int FinalMarker = paceChartRows.Count;
                double FinalDistance = (double)userInput.Distance - Math.Floor((double)userInput.Distance);
                TimeSpan FinalPace = (TimeSpan) userInput.Pace;
                TimeSpan FinalCummulativeTime = (TimeSpan)userInput.TotalTime;
                paceChartRows.Add(new PaceChart(FinalMarker, FinalDistance, FinalPace, FinalCummulativeTime));
                return paceChartRows;
            }
        }
        
        public static TimeSpan GradeAdjustedPace(TimeSpan pace, double paceAdjustment)
        {
            TimeSpan gradeAdjustedPace = pace.Multiply(paceAdjustment);
            
            return gradeAdjustedPace;
        }

        public static double CalculatePaceAdjustment(double grade)
        {
            //constants for polynomial equation for PaceAdjustment
            double a4 = -533.33;
            double a3 = 26.67;
            double a2 = 25.33;
            double a1 = 2.73;
            double a0 = 1.0;
            //variable for polynomial equation
            double x = Math.Max(-0.1, Math.Min(grade, 0.1));

            double PaceAdjustment = a4 * Math.Pow(x, 4) + a3 * Math.Pow(x, 3) + a2 * Math.Pow(x, 2) + a1 * x + a0;
            return PaceAdjustment;
        }
    }
}