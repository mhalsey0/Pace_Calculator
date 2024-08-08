using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RolandK.Formats.Gpx;

namespace Pace_Calculator.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int PaceHours { get; set; }
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

        }
        public async Task<IActionResult> OnPostAsync()
        {
        if (!ModelState.IsValid)
        {
            return Page();
        }            
        ModelState.Clear();

        await GPXUploadAsync();
        
        UserInput userInput = new()
        {
            Pace = new TimeSpan(PaceHours, PaceMinutes, PaceSeconds),
            Distance = InputDistance,
            TotalTime = new TimeSpan(TotalHours, TotalMinutes, TotalSeconds),
            Unit = Unit,
            GpxFileFromUser = GpxFileFromUser
        };

        if (userInput.GpxFileFromUser != null && userInput.TotalTime > TimeSpan.Zero)
        {
            var cleanFileName = Path.GetFileNameWithoutExtension(GpxFileFromUser.FileName);
            var fileExtension = Path.GetExtension(GpxFileFromUser.FileName);
            var safeFileName = $"{cleanFileName}_{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine(_environment.WebRootPath, "FileUpload");
            var gpxFile = await GpxFile.LoadAsync(uploadPath);


            
            System.Console.WriteLine($"Your file name is {safeFileName} and the path is {uploadPath}");
        }

        CalculatedInput calculatedInput = Calculators.Calculate(userInput);

        PaceHours = calculatedInput.Pace.Hours;
        PaceMinutes = calculatedInput.Pace.Minutes;
        PaceSeconds = calculatedInput.Pace.Seconds;
        InputDistance = calculatedInput.Distance;
        TotalHours = calculatedInput.TotalTime.Hours;
        TotalMinutes = calculatedInput.TotalTime.Minutes;
        TotalSeconds = calculatedInput.TotalTime.Seconds;
        
        PaceCharts = Calculators.CalculatePaceChart(calculatedInput);

        return Page();
        }
        private async Task GPXUploadAsync()
        {
            if (GpxFileFromUser == null || GpxFileFromUser.Length == 0)
            {
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
                
                using (var stream = new FileStream(filePath, FileMode.Create))
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