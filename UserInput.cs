namespace Pace_Calculator
{
    public class UserInput
    {
        public TimeSpan Pace { get; set; }
        public double Distance { get; set; }
        public TimeSpan TotalTime{ get; set; }
        public UnitOfLength Unit { get; set; }
        public IFormFile GpxFileFromUser { get; set; }
    }
}