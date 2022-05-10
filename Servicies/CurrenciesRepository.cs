using DSRProject.Data;
using DSRProject.Models;

namespace DSRProject.Servicies;
public interface ICurrenciesRepository {
    List<Currency> GetCurrencies();
    List<DateTime> GetDates(string currencyId, DateTime firstDate, DateTime secondDate);
    List<Course> GetCourses(string currencyId, DateTime firstDate, DateTime secondDate);
}
public class CurrenciesRepository : ICurrenciesRepository {
    private readonly CurrencyDbContext _context;
    public CurrenciesRepository(CurrencyDbContext context) => _context = context;
    public List<Currency> GetCurrencies() {
        return _context.Currencies.ToList();
    }
    public List<DateTime> GetDates(string currencyId, DateTime firstDate, DateTime secondDate) {
        return _context.Courses
            .Where(c => c.Currency.CurrencyId == currencyId)
            .Where(v => (v.Date > firstDate) && (v.Date < secondDate))
            .Select(e => e.Date)
            .OrderByDescending(o => o.Date).ToList();
    }
    public List<Course> GetCourses(string currencyId, DateTime firstDate, DateTime secondDate) {
        var coursesInDb = _context.Courses
            .Where(c => c.Currency.CurrencyId == currencyId)
            .Where(v => (v.Date > firstDate) && (v.Date < secondDate))
            .OrderByDescending(o => o.Date).ToList();
        var listOfValutes = _context.Currencies
            .Where(c => c.CurrencyId == currencyId).ToList();
        foreach(var course in coursesInDb) {
            course.Currency = listOfValutes.Where(c => c.CurrencyId == course.Currency.CurrencyId).First();
        }
        return coursesInDb;
    }
}