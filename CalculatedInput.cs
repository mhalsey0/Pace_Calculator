using System.ComponentModel.DataAnnotations;

namespace Pace_Calculator
{
    //class that holds results from calculations performed on UserInput class. This is an example of Single Responsiblity Princile in SOLID.
    public class CalculatedInput
    {
        public TimeSpan Pace { get; set; } = TimeSpan.FromSeconds(0);
        public double Distance { get; set; } = 0;
        public TimeSpan TotalTime{ get; set; } = TimeSpan.FromSeconds(0);
        [Required]
        public string Unit { get; set; }

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