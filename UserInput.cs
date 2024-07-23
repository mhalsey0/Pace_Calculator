namespace Pace_Calculator
{
    public class UserInput
    {
        public TimeSpan? Pace { get; set; }
        public double? Distance { get; set; }
        public TimeSpan? TotalTime{ get; set; }
        public string? Unit { get; set; }
        public string? GpxFile { get; set; }
    }
}
//may possibly need to add a Pace class made up for hours, minutes, seconds.
//this may make it easier to display results in their respective fields
//unsure if needs to be a timespan type