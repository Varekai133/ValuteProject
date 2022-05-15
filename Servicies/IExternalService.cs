namespace DSRProject.Servicies;
public interface IExternalService {
    Dictionary<float, DateTime> GetCourses(string currencyId, List<DateTime> listOfDates, DateTime firstDate, DateTime secondDate);
}
