
interface ICalculators
{
    //Will need to account for units in formatting for display
    public static TimeSpan PaceCalculator(string userInputTotalHours, string userInputTotalMinutes, string userInputTotalSeconds, string userInputTotalDistance)
    {
        double.TryParse(userInputTotalHours, out double totalHours);
        double.TryParse(userInputTotalMinutes, out double totalMinutes);
        double.TryParse(userInputTotalSeconds, out double totalSeconds);
        double.TryParse(userInputTotalDistance, out double totalDistance);
        
        TimeSpan pace = (TimeSpan.FromHours(totalHours) + 
                         TimeSpan.FromMinutes(totalMinutes) + 
                         TimeSpan.FromSeconds(totalSeconds)) 
                         /
                         totalDistance;

        return pace; //pace is equal to total time divided by total distance
    }
    public static double DistanceCalculator(string userInputTotalHours, string userInputTotalMinutes, string userInputTotalSeconds, string userInputPaceHours, string userInputPaceMinutes, string userInputPaceSeconds)
    {
        double.TryParse(userInputTotalHours, out double totalHours);
        double.TryParse(userInputTotalMinutes, out double totalMinutes);
        double.TryParse(userInputTotalSeconds, out double totalSeconds);
        double.TryParse(userInputPaceHours, out double paceHours);
        double.TryParse(userInputPaceMinutes, out double paceMinutes);
        double.TryParse(userInputPaceSeconds, out double paceSeconds);
        
        double distance = ( TimeSpan.FromHours(totalHours) +
                            TimeSpan.FromMinutes(totalMinutes) +
                            TimeSpan.FromSeconds(totalSeconds))
                            / (
                            TimeSpan.FromHours(paceHours) + 
                            TimeSpan.FromMinutes(paceMinutes) +
                            TimeSpan.FromSeconds(paceSeconds)); 

        return distance; //distance is equal to total time divided  by pace
    }
    public static TimeSpan TotalTimeCalculator(string userInputPaceHours, string userInputPaceMinutes, string userInputPaceSeconds, string userInputTotalDistance)
    {
        double.TryParse(userInputPaceHours, out double paceHours);
        double.TryParse(userInputPaceMinutes, out double paceMinutes);
        double.TryParse(userInputPaceSeconds, out double paceSeconds);
        double.TryParse(userInputTotalDistance, out double totalDistance);
        
        TimeSpan totalTime =  (TimeSpan.FromHours(paceHours) + 
                             TimeSpan.FromMinutes(paceMinutes) +
                             TimeSpan.FromSeconds(paceSeconds))
                             *
                             totalDistance;

        return totalTime; //total time is equal to pace multiplied by distance
    }
}
