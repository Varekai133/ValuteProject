using System.Text;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DSRProject.Data;
using DSRProject.Models;
using DSRProject.Servicies;
using DSRProject.Data.DTO;

namespace DSRProject.Pages;

public class IndexModel : PageModel
{
    private readonly CurrencyDbContext _context;
    private HttpClient _client;
    public IEnumerable<CurrencyDTO> Currencies { get; set; } = Enumerable.Empty<CurrencyDTO>();
    public List<List<CourseDTO>> ListOfCourses { get; set; } = new();
    [BindProperty]
    public DateTime FirstDate { get; set; }
    [BindProperty]
    public DateTime SecondDate { get; set; }
    [BindProperty]
    public List<string> checkedCheckboxes { get; set; }
    private readonly ICurrenciesRepository _currenciesRepository;
    private readonly IExternalService _externalService;
    public IndexModel(CurrencyDbContext context, HttpClient client, ICurrenciesRepository currenciesRepository, IExternalService externalService) {
        _context = context;
        _client = client;
        _currenciesRepository = currenciesRepository;
        _externalService = externalService;
    }
    public void OnGet() {
        Currencies = _currenciesRepository.GetCurrencies();
        SecondDate = DateTime.UtcNow.Date;
        FirstDate = SecondDate.AddDays(-30);
    }
    public void OnPost() {
        Currencies = _currenciesRepository.GetCurrencies();
        GetCoursesByCurrency();
    }
    public void GetCoursesByCurrency() {
        foreach (var ch in checkedCheckboxes) {
            GetRequiredCourses(ch);
        }
    }
    public void GetRequiredCourses(string currencyId) {
        var listOfDates = _currenciesRepository.GetDates(currencyId, FirstDate, SecondDate);
        try {
            var courses = _externalService.GetCourses(currencyId, listOfDates, FirstDate, SecondDate);
            _currenciesRepository.SaveCourses(currencyId, courses);
        }
        catch (Exception ex) {
            // exception
        }
        var coursesInDb = _currenciesRepository.GetCourses(currencyId, FirstDate, SecondDate);
        ListOfCourses.Add(coursesInDb);
    }
}