using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pace_Calculator.Pages;
public class IndexModel : PageModel
{
    public UserInput? UserInput { get; set; }
    public int paceHours { get; set; }
    public int paceMinutes { get; set; }
    public int paceSeconds { get; set; }
    public int totalHours { get; set; }
    public int totalMinutes { get; set; }
    public int totalSeconds { get; set; }

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
    public void OnPost()
    {
        UserInput.Pace = new TimeSpan(paceHours, paceMinutes, paceSeconds);
        UserInput.TotalTime = new TimeSpan(totalHours, totalMinutes,totalSeconds);

        Calculators.Calculate(UserInput);
        return;
    }
}
