using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RolandK.Formats.Gpx;

namespace Pace_Calculator.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty, Description("Hours")]
        public int PaceHours { get; set; }
        
        [BindProperty, Description("Minutes")]
        public int PaceMinutes { get; set; }
        
        [BindProperty, Description("Seconds")]
        public int PaceSeconds { get; set; }
        
        [BindProperty, Description("Distance"),Range(0.01, double.MaxValue, ErrorMessage = "Distance must be greater than 0.")]
        public double InputDistance { get; set; }
        
        [BindProperty, Description("Hours")]
        public int TotalHours { get; set; }
        
        [BindProperty, Description("Minutes")]
        public int TotalMinutes { get; set; }
        
        [BindProperty, Description("Seconds")]
        public int TotalSeconds { get; set; }
        
        [BindProperty, Required]
        public UnitOfLength Unit { get; set; }
        
        [BindProperty]
        public IFormFile? GpxFileFromUser { get; set; }
        
        public List<PaceChart> PaceCharts { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnGet()
        {
            // Removed unnecessary method if not used.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid.");
                return Page();
            }

            try
            {
                _logger.LogInformation("Processing user input.");
                
                ModelState.Clear();

                TimeSpan totalTime = new TimeSpan(TotalHours, TotalMinutes, TotalSeconds);
                TimeSpan pace = new TimeSpan(PaceHours, PaceMinutes, PaceSeconds);

                // Added null check before accessing GpxFileFromUser properties
                if (GpxFileFromUser != null)
                {
                    await GPXUploadAsync();
                    var filePath = Path.Combine(_environment.WebRootPath, "FileUpload", GpxFileFromUser.FileName);
                    GpxFile gpxFile;
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        gpxFile = GpxFile.Load(fileStream);
                    }
                    var distance = Calculators.GetTotalDistanceFromGpxFile(gpxFile, Unit);

                    // Refactored repeated code into a method to avoid duplication
                    if (totalTime != TimeSpan.Zero)
                    {
                        TimeSpan pace1 = Calculators.PaceCalculator(totalTime,distance);
                    
                        UserInput userInput1 = new()
                        {
                            Pace = pace1,
                            Distance = distance,
                            TotalTime = totalTime,
                            Unit = Unit,
                            GpxFileFromUser = GpxFileFromUser
                        };
                        CalculatedInput calculatedInput1 = CalculatedInput.FromUserInput(userInput1);
                        UpdateProperties(calculatedInput1);
                        var gradeAdjustedPaceChart = Calculators.CalculateGradeAdjustedPaceChart(gpxFile, calculatedInput1);                        
                    }

                    if (pace != TimeSpan.Zero)
                    {
                        TimeSpan totalTime1 = Calculators.TotalTimeCalculator(pace, distance);
                        
                        UserInput userInput1 = new()
                        {
                            Pace = pace,
                            Distance = distance,
                            TotalTime = totalTime1,
                            Unit = Unit,
                            GpxFileFromUser = GpxFileFromUser
                        };
                        CalculatedInput calculatedInput1 = CalculatedInput.FromUserInput(userInput1);
                        UpdateProperties(calculatedInput1);
                        var gradeAdjustedPaceChart = Calculators.CalculateGradeAdjustedPaceChart(gpxFile, calculatedInput1); 
                    }
                }
                else
                {
                    // Process calculation without GPX file
                    UserInput userInput = new()
                    {
                        Pace = pace,
                        Distance = InputDistance,
                        TotalTime = totalTime,
                        Unit = Unit,
                        GpxFileFromUser = GpxFileFromUser
                    };
                    CalculatedInput calculatedInput = Calculators.Calculate(userInput);

                    UpdateProperties(calculatedInput);

                    PaceCharts = Calculators.CalculatePaceChart(calculatedInput);

                    _logger.LogInformation("Processing completed successfully.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during post processing.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return Page();
            }

            return Page();
        }

        private void UpdateProperties(CalculatedInput calculatedInput)
        {
            PaceHours = calculatedInput.Pace.Hours;
            PaceMinutes = calculatedInput.Pace.Minutes;
            PaceSeconds = calculatedInput.Pace.Seconds;
            InputDistance = calculatedInput.Distance;
            TotalHours = calculatedInput.TotalTime.Hours;
            TotalMinutes = calculatedInput.TotalTime.Minutes;
            TotalSeconds = calculatedInput.TotalTime.Seconds;
        }

        private async Task GPXUploadAsync()
        {
            if (GpxFileFromUser == null || GpxFileFromUser.Length == 0)
            {
                _logger.LogWarning("No file uploaded or file is empty.");
                return;
            }

            try
            {
                _logger.LogInformation($"Uploading {GpxFileFromUser.FileName}.");
                
                var cleanFileName = Path.GetFileNameWithoutExtension(GpxFileFromUser.FileName);
                var fileExtension = Path.GetExtension(GpxFileFromUser.FileName);
                var safeFileName = $"{cleanFileName}_{Guid.NewGuid()}{fileExtension}";
                
                var uploadPath = Path.Combine(_environment.WebRootPath, "FileUpload");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, safeFileName);
                
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await GpxFileFromUser.CopyToAsync(stream);
                }

                _logger.LogInformation($"File uploaded successfully to {filePath}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
            }
        }
    }
}
