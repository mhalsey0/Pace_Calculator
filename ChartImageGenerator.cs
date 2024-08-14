using System.Reflection.Metadata.Ecma335;
using ImageChartsLib;
using RolandK.Formats.Gpx;

namespace Pace_Calculator
{
    public class ChartImageGenerator
    {
        public static string GenerateChartImage(GpxFile gpxFile)
        {
            // Ensure gpxFile is not null
            if (gpxFile == null)
            {
                throw new ArgumentNullException(nameof(gpxFile));
            }

            List<double> elevations = Calculators.GetElevationPoints(gpxFile);

            if (elevations == null || elevations.Count == 0)
            {
                throw new InvalidOperationException("No elevation points found in the GPX file.");
            }

            while (elevations.Count > 1200)
            {
                elevations = HalveListofElevations(elevations);
            }

            // Convert the elevations list to a comma-separated string
            string chartString = "a:" + string.Join(",", elevations);

            string chartUrl = new ImageCharts()
                .cht("lc") // indicates type of graph: line
                .chs("900x200") // size
                .chd(chartString) // data points
                .toURL(); // get the generated URL

            
            return chartUrl;
        }

        public static List<double> HalveListofElevations(List<double> elevations)
        {
            List<double> shortenedElevationList = new List<double>();
            for (int i = 0; i < elevations.Count; i++)
            {
                if (i % 2 != 0)
                {
                    shortenedElevationList.Add(elevations[i]);
                }
            }
            return shortenedElevationList;
        }
    }
}
