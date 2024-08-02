using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pace_Calculator.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int PaceMinutes { get; set; }
        [BindProperty]
        public int PaceSeconds { get; set; }
        [BindProperty]
        public double InputDistance { get; set; }
        [BindProperty]
        public int TotalHours { get; set; }
        [BindProperty]
        public int TotalMinutes { get; set; }
        [BindProperty]
        public int TotalSeconds { get; set; }
        [BindProperty]
        [Required]
        public required string Unit { get; set; }
        [BindProperty, Display(Name = "Course")]
        public IFormFile? GpxFile { get; set; }
        public List<PaceChart> paceCharts { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly IHostEnvironment _environment;

        public IndexModel(ILogger<IndexModel> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            ModelState.Clear();
            UserInput userInput = new()
            {
                Pace = new TimeSpan( 0,PaceMinutes, PaceSeconds),
                Distance = InputDistance,
                TotalTime = new TimeSpan(TotalHours, TotalMinutes, TotalSeconds),
                Unit = Unit
            };
            CalculatedInput calculatedInput = new()
            {
                Pace = new TimeSpan(0,0,0),
                Distance = 0,
                TotalTime = new TimeSpan(0,0,0)
            };

            Calculators.Calculate(userInput, calculatedInput);
            
            
            PaceMinutes = calculatedInput.Pace.Minutes;
            PaceSeconds = calculatedInput.Pace.Seconds;
            InputDistance = calculatedInput.Distance;
            TotalHours = calculatedInput.TotalTime.Hours;
            TotalMinutes = calculatedInput.TotalTime.Minutes;
            TotalSeconds = calculatedInput.TotalTime.Seconds;
            
            List<PaceChart> paceChart = new List<PaceChart>();
            
            paceCharts = Calculators.CalculatePaceChart(userInput);

            Console.WriteLine(userInput.Pace.ToString(), userInput.Distance.ToString(), userInput.TotalTime.ToString());

            return Page();
        }
        public async Task OnPostGPXUploadAsync()
        {
            if (GpxFile == null || GpxFile.Length == 0)
            {
                return;
            }
 
            _logger.LogInformation($"Uploading {GpxFile.FileName}.");
            string targetFileName = $"{_environment.ContentRootPath}/{GpxFile.FileName}";
 
            using (var stream = new FileStream(targetFileName, FileMode.Create))
            {
                await GpxFile.CopyToAsync(stream);
            }
        }
    }
}
