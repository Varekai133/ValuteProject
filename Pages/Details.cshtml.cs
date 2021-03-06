using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DSRProject.Data;
using DSRProject.Servicies;
using DSRProject.Data.DTO;

namespace DSRProject.Pages;

public class DetailsModel : PageModel
{
    private readonly CurrencyDbContext _context;
    private HttpClient _client;
    private readonly ICurrenciesRepository _currenciesRepository;
    private readonly IExternalService _externalService;

    public IEnumerable<CourseDTO> Courses { get; set; } = Enumerable.Empty<CourseDTO>();
    [BindProperty]
    public DateTime FirstDate { get; set; }
    [BindProperty]
    public DateTime SecondDate { get; set; }
    public string Message { get; set; }

    public DetailsModel(CurrencyDbContext context, HttpClient client, ICurrenciesRepository currenciesRepository, IExternalService externalService) {
        _context = context;
        _client = client;
        _currenciesRepository = currenciesRepository;
        _externalService = externalService;
    }

    public void OnGet(string currencyId) {
        SecondDate = DateTime.UtcNow.Date;
        FirstDate = SecondDate.AddDays(-30);
        GetRequiredCourses(currencyId);
    }

    public void OnPost(string currencyId) {
        GetRequiredCourses(currencyId);
    }

    public void GetRequiredCourses(string currencyId) {
        var listOfDates = _currenciesRepository.GetDates(currencyId, FirstDate, SecondDate);
        try {
            var courses = _externalService.DownloadCourses(currencyId, listOfDates, FirstDate, SecondDate);
            _currenciesRepository.SaveCourses(currencyId, courses);
        }
        catch (Exception ex) {
            Message = ex.Message;
        }
        var coursesInDb = _currenciesRepository.GetCourses(currencyId, FirstDate, SecondDate);
        Courses = coursesInDb;
    }
}