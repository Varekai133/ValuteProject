using System.Text;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DSRProject.Data;
using DSRProject.Models;
using DSRProject.Servicies;

namespace DSRProject.Pages;

public class DetailsModel : PageModel
{
    private readonly CurrencyDbContext _context;
    private HttpClient _client;
    public IEnumerable<Course> Courses { get; set; } = Enumerable.Empty<Course>();
    [BindProperty]
    public DateTime FirstDate { get; set; }
    [BindProperty]
    public DateTime SecondDate { get; set; }
    private readonly ICurrenciesRepository _currenciesRepository;
    public DetailsModel(CurrencyDbContext context, HttpClient client, ICurrenciesRepository currenciesRepository) {
        _context = context;
        _client = client;
        _currenciesRepository = currenciesRepository;
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
            DowloandNewCourses(currencyId, listOfDates);
        }
        catch (Exception ex) {
            // exception
        }
        var coursesInDb = _currenciesRepository.GetCourses(currencyId, FirstDate, SecondDate);
        Courses = coursesInDb;
    }
    public void DowloandNewCourses(string currencyId, List<DateTime> listOfDates) {
        string firstDateCourse = FirstDate.ToString("dd.MM.yyyy");
        string secondDateCourse = SecondDate.ToString("dd.MM.yyyy");
        var response = _client.GetAsync($"https://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={firstDateCourse}&date_req2={secondDateCourse}&VAL_NM_RQ={currencyId}").Result;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding.GetEncoding("windows-1254");
        var result = response.Content.ReadAsStringAsync().Result;
        var xml = XDocument.Parse(result);

        var xmlValuesList = xml.Descendants("Value").ToList();
        var xmlDatesList = xml.Descendants("Record").Attributes("Date").ToList();
        var xmlNominalsList = xml.Descendants("Nominal").ToList();

        Dictionary<float, DateTime> resultDictionarty = new();
        for (int i = 0; i < xmlDatesList.Count(); i++) {
            if (!listOfDates.Contains(DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date))
                resultDictionarty.Add(float.Parse(xmlValuesList.ElementAt(i).Value) / float.Parse(xmlNominalsList.ElementAt(i).Value), DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date);
        }

        var courses = new Course[resultDictionarty.Count()];
        for (int i = 0; i < resultDictionarty.Count(); i++) {
            courses[i] = new Course {
                Value = resultDictionarty.Keys.ElementAt(i),
                Date = resultDictionarty.Values.ElementAt(i),
                Currency = _context.Currencies.Where(i => i.CurrencyId == currencyId).First()
            };
        }
        _context.AddRange(courses);
        _context.SaveChanges();
    }
}