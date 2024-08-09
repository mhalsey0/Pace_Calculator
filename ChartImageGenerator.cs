using System.Reflection.Metadata.Ecma335;
using ImageChartsLib;
using RolandK.Formats.Gpx;

namespace Pace_Calculator
{
    public class ChartImageGenerator
    {
        public static string GenerateChartImage(GpxFile gpxFile)
        {
            List<double> elevations = Calculators.GetElevationPoints(gpxFile);
            string chartString = string.Join(",","a:",elevations);

            string chartUrl = new ImageCharts()
                .cht("lc") // line chart
                .chs("800x300") // 300px x 300px
                .chd(chartString) // 2 data points: 60 and 40
                .toURL(); // get the generated URL

            return chartUrl;
        }
    }
}