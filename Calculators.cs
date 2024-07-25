namespace Pace_Calculator
{
    public class Calculators
    {
        public static void Calculate(UserInput userInput)
        {
            if(userInput.Pace == TimeSpan.FromHours(0))
            {
                userInput.Pace = PaceCalculator((TimeSpan)userInput.TotalTime, (double)userInput.Distance);
            }
            if(userInput.Distance == 0)
            {
                userInput.Distance = DistanceCalculator((TimeSpan)userInput.TotalTime, (TimeSpan)userInput.Pace);
            }
            if (userInput.TotalTime == TimeSpan.FromHours(0))
            {
                userInput.TotalTime = TotalTimeCalculator((TimeSpan)userInput.Pace, (double)userInput.Distance);
            }
            return;
        }
        //Will need to account for units in formatting for display
        public static TimeSpan PaceCalculator(TimeSpan totalTime, double distance)
        {
            TimeSpan pace = totalTime / distance;
            return pace; //pace is equal to total time divided by total distance
        }
        public static double DistanceCalculator(TimeSpan totalTime, TimeSpan pace)
        {        
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
    }
}