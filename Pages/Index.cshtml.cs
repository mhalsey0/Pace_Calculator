using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        public required string Unit { get; set; }
        [BindProperty]
        public string? GpxFile { get; set; }


/*        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
*/
        public void OnGet()
        {

        }
        public void OnPost()
        {
            UserInput userInput = new()
            {
                Pace = new TimeSpan(PaceHours, PaceMinutes, PaceSeconds),
                Distance = InputDistance,
                TotalTime = new TimeSpan(TotalHours, TotalMinutes, TotalSeconds),
                Unit = Unit
            };
            Calculators.Calculate(userInput);
            //InputDistance = (double)userInput.Distance;

            Console.WriteLine($"{userInput.TotalTime},{userInput.Pace},{userInput.Distance}");
            return;
        }
    }
}
