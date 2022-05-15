using DSRProject.Data;
using DSRProject.Data.DTO;
using DSRProject.Extensions;
using DSRProject.Models;

namespace DSRProject.Servicies;
public interface ICurrenciesRepository {
    List<CurrencyDTO> GetCurrencies();
    List<DateTime> GetDates(string currencyId, DateTime firstDate, DateTime secondDate);
    List<CourseDTO> GetCourses(string currencyId, DateTime firstDate, DateTime secondDate);
    void SaveCourses(string currencyId, Dictionary<float, DateTime> coursesDictionarty);
}