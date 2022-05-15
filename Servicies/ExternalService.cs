using System.Text;
using System.Xml.Linq;
using DSRProject.Data;
using DSRProject.Models;

namespace DSRProject.Servicies;
public class ExternalService : IExternalService {
    private readonly CurrencyDbContext _context;
    private readonly HttpClient _client;
    public ExternalService(CurrencyDbContext context, HttpClient client) {
        _context = context;
        _client = client;
    }

    public Dictionary<float, DateTime> GetCourses(string currencyId, List<DateTime> listOfDates, DateTime firstDate, DateTime secondDate) {
        string firstDateCourse = firstDate.ToString("dd.MM.yyyy");
        string secondDateCourse = secondDate.ToString("dd.MM.yyyy");
        var response = _client.GetAsync($"https://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={firstDateCourse}&date_req2={secondDateCourse}&VAL_NM_RQ={currencyId}").Result;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding.GetEncoding("windows-1254");
        var result = response.Content.ReadAsStringAsync().Result;
        var xml = XDocument.Parse(result);

        var xmlValuesList = xml.Descendants("Value").ToList();
        var xmlDatesList = xml.Descendants("Record").Attributes("Date").ToList();
        var xmlNominalsList = xml.Descendants("Nominal").ToList();

        Dictionary<float, DateTime> coursesDictionarty = new();
        for (int i = 0; i < xmlDatesList.Count(); i++) {
            if (!listOfDates.Contains(DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date))
                coursesDictionarty.Add(float.Parse(xmlValuesList.ElementAt(i).Value) / float.Parse(xmlNominalsList.ElementAt(i).Value), DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date);
        }
        return coursesDictionarty;
        // var courses = new Course[resultDictionarty.Count()];
        // for (int i = 0; i < resultDictionarty.Count(); i++) {
        //     courses[i] = new Course {
        //         Value = resultDictionarty.Keys.ElementAt(i),
        //         Date = resultDictionarty.Values.ElementAt(i),
        //         Currency = _context.Currencies.Where(i => i.CurrencyId == currencyId).First()
        //     };
        // }
        // _context.AddRange(courses);
        // _context.SaveChanges();
    }
}