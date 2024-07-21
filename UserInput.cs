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