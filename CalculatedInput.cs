namespace Pace_Calculator
{
    public class CalculatedInput
    {
        public TimeSpan Pace { get; set; } = TimeSpan.FromSeconds(0);
        public double Distance { get; set; } = 0;
        public TimeSpan TotalTime{ get; set; } = TimeSpan.FromSeconds(0);
        public UnitOfLength Unit { get; set; }

        public CalculatedInput()
        {
            
        }

        public static CalculatedInput FromUserInput(UserInput userInput) => new()
        {
            Pace = userInput.Pace,
            Distance = userInput.Distance,
            TotalTime = userInput.TotalTime,
            Unit = userInput.Unit,
        };
    }
}