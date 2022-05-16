using System.Text;
using System.Xml.Linq;

namespace DSRProject.Servicies;

public class ExternalService : IExternalService {
    private readonly HttpClient _client;   

    public ExternalService(HttpClient client) {
        _client = client;
    }

    public Dictionary<DateTime, float> DownloadCourses(string currencyId, List<DateTime> listOfDates, DateTime firstDate, DateTime secondDate) {
        if (firstDate >= secondDate) {
            throw new Exception("Проверьте правильность введенных дат");
        }

        string firstDateCourse = firstDate.ToString("dd.MM.yyyy");
        string secondDateCourse = secondDate.ToString("dd.MM.yyyy");
        var response = _client.GetAsync($"https://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={firstDateCourse}&date_req2={secondDateCourse}&VAL_NM_RQ={currencyId}").Result;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding.GetEncoding("windows-1254");
        var result = response.Content.ReadAsStringAsync().Result;

        XDocument xml = new();
        try {
            xml = XDocument.Parse(result);
        }
        catch (Exception ex) {
            throw new Exception("Невозможно загрузить новые данные");
        }

        var xmlValuesList = xml.Descendants("Value").ToList();
        var xmlDatesList = xml.Descendants("Record").Attributes("Date").ToList();
        var xmlNominalsList = xml.Descendants("Nominal").ToList();

        Dictionary<DateTime, float> coursesDictionarty = new();
        for (int i = 0; i < xmlDatesList.Count(); i++) {
            if (!listOfDates.Contains(DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date))
                coursesDictionarty.Add(DateTime.Parse(xmlDatesList.ElementAt(i).Value).Date, float.Parse(xmlValuesList.ElementAt(i).Value) / float.Parse(xmlNominalsList.ElementAt(i).Value));
        }
        return coursesDictionarty;
    }
}