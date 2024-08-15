namespace Pace_Calculator
{
    //Class to hold and format UserInput
    public class UserInput
    {
        public TimeSpan Pace { get; set; }
        public double Distance { get; set; }
        public TimeSpan TotalTime{ get; set; }
        public string Unit { get; set; }
        public IFormFile? GpxFileFromUser { get; set; }
    }
}