namespace Pace_Calculator
{
    public class PreviousInput
    {
        public TimeSpan Pace { get; set; } = TimeSpan.FromSeconds(0);
        public double Distance { get; set; } = 0;
        public TimeSpan TotalTime{ get; set; } = TimeSpan.FromSeconds(0);
        public required string Unit { get; set; } = "";
        public string? GpxFile { get; set; } = null;


        public PreviousInput()
        {
            
        }

        public static PreviousInput FromUserInput(UserInput userInput) => new PreviousInput
        {
            Pace = userInput.Pace,
            Distance = userInput.Distance,
            TotalTime = userInput.TotalTime,
            Unit = userInput.Unit,
            GpxFile = userInput.GpxFile
        };
    }
}