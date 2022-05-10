using System.Linq;
using System.Text;
using DSRProject.Data;
using DSRProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
// using Windows.Storage.Streams;

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
    public DetailsModel(CurrencyDbContext context, HttpClient client) {
        _context = context;
        _client = client;
    }
    public void OnGet(string valuteId) {
        SecondDate = DateTime.UtcNow.Date;
        FirstDate = SecondDate.AddDays(-30);
        GetRequiredCourses(valuteId);
    }
    public void OnPost(string valuteId) {
        GetRequiredCourses(valuteId);
    }
    public void GetRequiredCourses(string valuteId) {
        var listOfDates = _context.Courses
            .Where(c => c.Currency.CurrencyId == valuteId)
            .Where(v => (v.Date > FirstDate) && (v.Date < SecondDate))
            .Select(e => e.Date)
            .OrderByDescending(o => o.Date).ToList();
        try {
            DowloandNewCourses(valuteId, listOfDates);
        }
        catch (Exception ex) {
            // exception
        }
        var coursesInDb = _context.Courses
            .Where(c => c.Currency.CurrencyId == valuteId)
            .Where(v => (v.Date > FirstDate) && (v.Date < SecondDate))
            .OrderByDescending(o => o.Date).ToList();
        var listOfValutes = _context.Currencies
            .Where(c => c.CurrencyId == valuteId).ToList();
        foreach(var course in coursesInDb) {
            course.Currency = listOfValutes.Where(c => c.CurrencyId == course.Currency.CurrencyId).First();
        }
        Courses = coursesInDb;
    }
    public void DowloandNewCourses(string valuteId, List<DateTime> listOfDates) {
        string firstDateCourse = FirstDate.ToString("dd.MM.yyyy");
        string secondDateCourse = SecondDate.ToString("dd.MM.yyyy");
        var response = _client.GetAsync($"https://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={firstDateCourse}&date_req2={secondDateCourse}&VAL_NM_RQ={valuteId}").Result;

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
                Currency = _context.Currencies.Where(i => i.CurrencyId == valuteId).First()
            };
        }
        _context.AddRange(courses);
        _context.SaveChanges();
    }
}