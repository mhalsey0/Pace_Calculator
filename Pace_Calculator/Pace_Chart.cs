using System.Collections.Generic;

public class Pace_Chart{
    public string Unit { get; set; }
    public int Distance { get; set; }
    public int Pace { get; set; }   
    public List<Distance_Marker> Markers { get; set; }
}