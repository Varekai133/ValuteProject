namespace DSRProject.Servicies;

public interface IExternalService {
    Dictionary<DateTime, float> DownloadCourses(string currencyId, List<DateTime> listOfDates, DateTime firstDate, DateTime secondDate);
}