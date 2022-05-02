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
    private readonly ValuteDbContext _context;
    private HttpClient _client;
    public IEnumerable<Course> Courses { get; set; } = Enumerable.Empty<Course>();
    [BindProperty]
    public DateTime firstDate { get; set; }
    [BindProperty]
    public DateTime secondDate { get; set; }
    public DetailsModel(ValuteDbContext context, HttpClient client) {
        _context = context;
        _client = client;
    }
    public void OnGet(string valuteId) {
        secondDate = DateTime.UtcNow.Date;
        firstDate = secondDate.AddDays(-30);
        GetRequiredCourses(valuteId);
    }
    public void OnPost(string valuteId) {
        GetRequiredCourses(valuteId);
    }
    public void GetRequiredCourses(string valuteId) {
        var listOfDates = _context.Courses
            .Where(c => c.Valute.ValuteId == valuteId)
            .Where(v => (v.Date > firstDate) && (v.Date < secondDate))
            .Select(e => e.Date)
            .OrderByDescending(o => o.Date).ToList();
        try {
            DowloandNewCourses(valuteId, listOfDates);
        }
        catch (Exception ex) {
            // exception
        }
        var coursesInDb = _context.Courses
            .Where(c => c.Valute.ValuteId == valuteId)
            .Where(v => (v.Date > firstDate) && (v.Date < secondDate))
            .OrderByDescending(o => o.Date).ToList();
        var listOfValutes = _context.Valutes
            .Where(c => c.ValuteId == valuteId).ToList();
        foreach(var course in coursesInDb) {
            course.Valute = listOfValutes.Where(c => c.ValuteId == course.Valute.ValuteId).First();
        }
        Courses = coursesInDb;
    }
    public void DowloandNewCourses(string valuteId, List<DateTime> listOfDates) {
        string firstDateCourse = firstDate.ToString("dd.MM.yyyy");
        string secondDateCourse = secondDate.ToString("dd.MM.yyyy");
        var response = _client.GetAsync($"https://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={firstDateCourse}&date_req2={secondDateCourse}&VAL_NM_RQ={valuteId}").Result;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding.GetEncoding("windows-1254");
        var result = response.Content.ReadAsStringAsync().Result;
        var xml = XDocument.Parse(result);

        var xmlValuesList = xml.Descendants("Value").ToList();
        var xmlDatesList = xml.Descendants("Record").Attributes("Date").ToList();

        Dictionary<float, DateTime> resultDictionarty = new();
        for (int i = 0; i < xmlDatesList.Count(); i++) {
            if (!listOfDates.Contains(DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date))
                resultDictionarty.Add(float.Parse(xmlValuesList.ElementAt(i).Value), DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date);
        }

        var courses = new Course[resultDictionarty.Count()];
        for (int i = 0; i < resultDictionarty.Count(); i++) {
            courses[i] = new Course {
                Value = resultDictionarty.Keys.ElementAt(i),
                Date = resultDictionarty.Values.ElementAt(i),
                Valute = _context.Valutes.Where(i => i.ValuteId == valuteId).First()
            };
        }
        _context.AddRange(courses);
        _context.SaveChanges();
    }
}