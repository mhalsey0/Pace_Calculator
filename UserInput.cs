public class UserInput : ICalculators
{
    public double? PaceHours { get; set; }
    public double? PaceMinutes { get; set;}   
    public double? PaceSeconds { get; set;}
    public double? TotalHours { get; set; }
    public double? TotalMinutes { get; set; }
    public double? TotalSeconds { get; set; }
    public double? Distance { get; set; }
    public string? Unit { get; set; }
    public string? GpxFile { get; set; }
}