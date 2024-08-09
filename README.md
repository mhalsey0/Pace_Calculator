## Pace Calculator

## Description
A pace calculator that will take a .gpx file and either a total time or average 
pace and then provide a grade adjusted pace chart and an elevation profile image. 
If no .gpx file is provided then you may input two of three variables to find
the third. (e.g. total time and distance given will calculate the pace).

## Feature Overview

- Uses Image-Chart API to create the Elevation Profile. https://documentation.image-charts.com
- Takes advantage of Asynchronus methods to process the uploaded file.
- Creates several lists based on properties from the .gpx and accesses those lists for calculation.
- Uses Nuget packages: RolandK.Formats.Gpx ; ImageCharts ; popper.js.
- Web based application using Razor Pages

## How to Use

1. Load application and click the "Start Creating your Pace Chart" button
2. Upload your .gpx file (see 
