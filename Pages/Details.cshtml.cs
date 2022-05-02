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
    public DetailsModel(ValuteDbContext context, HttpClient client) {
        _context = context;
        _client = client;
    }
    public void OnGet(string valuteId) {
        var coursesInDb = _context.Courses.Where(c => c.Valute.ValuteId == valuteId)
            .OrderByDescending(o => o.Date).ToList();
        var listOfDates = _context.Courses.Where(c => c.Valute.ValuteId == valuteId).Select(e => e.Date)
            .OrderByDescending(o => o.Date).ToList();
        if (coursesInDb.Count() < 22) {
            try {
                DowloandNewCoursesByDefault(valuteId, listOfDates);
            }
            catch (Exception ex) {

            }
        }
        coursesInDb = _context.Courses.Where(c => c.Valute.ValuteId == valuteId)
            .OrderByDescending(o => o.Date).ToList();
        var t = _context.Valutes.Where(c => c.ValuteId == valuteId).ToList();
        foreach(var v in coursesInDb) {
            v.Valute = t.Where(c => c.ValuteId == v.Valute.ValuteId).First();
        }
    
        Courses = coursesInDb;
    }
    public void DowloandNewCoursesByDefault(string valuteId, List<DateTime> listOfDates) {
        DateTime currentDate = DateTime.UtcNow.Date;
        string firstDate = currentDate.ToString("dd.MM.yyyy");
        string secondDate = currentDate.AddDays(-30).ToString("dd.MM.yyyy");
        DowloandNewCourses(valuteId, listOfDates, firstDate, secondDate);
    }
    public void DowloandNewCourses(string valuteId, List<DateTime> listOfDates, string firstDate, string secondDate) {
        var response = _client.GetAsync($"https://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={secondDate}&date_req2={firstDate}&VAL_NM_RQ={valuteId}").Result;

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