
internal class Calculators
{
    //need to connect to input from UI
    string unit = "miles"; //can have option for km, meters, or yards as well
    double distance? = 3;
    double hours? = 0;
    double minutes? = 30;
    double seconds? = 0;
    double pace? = 0;

    public Calculators()
    {
        //formula is pace = total time / total units
        //will need to consider rounding and formatting
    }
}
