using DSRProject.Data.DTO;

namespace DSRProject.Servicies;
public interface ICurrenciesRepository {
    List<CurrencyDTO> GetCurrencies();
    List<DateTime> GetDates(string currencyId, DateTime firstDate, DateTime secondDate);
    List<CourseDTO> GetCourses(string currencyId, DateTime firstDate, DateTime secondDate);
    void SaveCourses(string currencyId, Dictionary<DateTime, float> coursesDictionarty);
}