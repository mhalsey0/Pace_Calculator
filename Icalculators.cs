interface ICalculators
{   
    TimeSpan TotalTimeCalculator(double paceHours, double paceMinutes, double paceSeconds, double totalDistance);
    TimeSpan PaceCalculator();
    double DistanceCalculator();
}