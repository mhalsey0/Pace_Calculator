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
    }
}