using DSRProject.Data;
using DSRProject.Data.DTO;
using DSRProject.Extensions;
using DSRProject.Models;

namespace DSRProject.Servicies;
public class CurrenciesRepository : ICurrenciesRepository {
    private readonly CurrencyDbContext _context;
    public CurrenciesRepository(CurrencyDbContext context) => _context = context;
    
    public List<CurrencyDTO> GetCurrencies() {
        return _context.Currencies.Select(item => item.AsDto()).ToList();
    }

    public List<DateTime> GetDates(string currencyId, DateTime firstDate, DateTime secondDate) {
        return _context.Courses
            .Where(c => c.Currency.CurrencyId == currencyId)
            .Where(v => (v.Date > firstDate) && (v.Date < secondDate))
            .Select(e => e.Date)
            .OrderByDescending(o => o.Date).ToList();
    }
    public List<CourseDTO> GetCourses(string currencyId, DateTime firstDate, DateTime secondDate) {
        var coursesInDb = _context.Courses
            .Where(c => c.Currency.CurrencyId == currencyId)
            .Where(v => (v.Date > firstDate) && (v.Date < secondDate))
            .OrderByDescending(o => o.Date).ToList();
        var listOfValutes = _context.Currencies
            .Where(c => c.CurrencyId == currencyId).ToList();
        foreach(var course in coursesInDb) {
            course.Currency = listOfValutes.Where(c => c.CurrencyId == course.Currency.CurrencyId).First();
        }
        return coursesInDb.Select(item => item.AsDto()).ToList();
    }

    public void SaveCourses(string currencyId, Dictionary<DateTime, float> coursesDictionarty) {
        var courses = new Course[coursesDictionarty.Count()];
        for (int i = 0; i < coursesDictionarty.Count(); i++) {
            courses[i] = new Course {
                Value = coursesDictionarty.Values.ElementAt(i),
                Date = coursesDictionarty.Keys.ElementAt(i),
                Currency = _context.Currencies.Where(i => i.CurrencyId == currencyId).First()
            };
        }
        _context.AddRange(courses);
        _context.SaveChanges();
    }
}