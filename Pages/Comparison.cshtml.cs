using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DSRProject.Data;
using DSRProject.Servicies;
using DSRProject.Data.DTO;

namespace DSRProject.Pages;
public class ComparisonModel : PageModel
{
    private readonly CurrencyDbContext _context;
    private HttpClient _client;
    private readonly ICurrenciesRepository _currenciesRepository;
    private readonly IExternalService _externalService;
    
    public List<string> SelectedCourses { get; set; } = new();
    public List<List<CourseDTO>> ListOfCourses { get; set; } = new();
    [BindProperty]
    public DateTime FirstDate { get; set; }
    [BindProperty]
    public DateTime SecondDate { get; set; }
    public string Message { get; set; }

    public ComparisonModel(CurrencyDbContext context, HttpClient client, ICurrenciesRepository currenciesRepository, IExternalService externalService) {
        _context = context;
        _client = client;
        _currenciesRepository = currenciesRepository;
        _externalService = externalService;
    }

    public void OnGet(string selectedCourses) {
        SelectedCourses = selectedCourses.Split('/').ToList();
        SecondDate = DateTime.UtcNow.Date;
        FirstDate = SecondDate.AddDays(-30);
        GetRequiredCourses();
    }

    public void OnPost(string selectedCourses) {
        SelectedCourses = selectedCourses.Split('/').ToList();
        GetRequiredCourses();
    }

    public void GetRequiredCourses() {
        foreach (var currency in SelectedCourses) {
            var listOfDates = _currenciesRepository.GetDates(currency, FirstDate, SecondDate);
            try {
                var courses = _externalService.DownloadCourses(currency, listOfDates, FirstDate, SecondDate);
                _currenciesRepository.SaveCourses(currency, courses);
            }
            catch (Exception ex) {
                Message = ex.Message;
            }
            var coursesInDb = _currenciesRepository.GetCourses(currency, FirstDate, SecondDate);
            ListOfCourses.Add(coursesInDb);
        }
    }
}