using DSRProject.Data.DTO;
using DSRProject.Models;

namespace DSRProject.Extensions;

public static class Extensions {
    public static CourseDTO AsDto(this Course course) {
        return new CourseDTO {
            CourseId = course.CourseId,
            Value = course.Value,
            Date = course.Date,
            Currency = course.Currency
        };
    }

    public static CurrencyDTO AsDto(this Currency currency) {
        return new CurrencyDTO {
            CurrencyId = currency.CurrencyId,
            NumCode = currency.NumCode,
            CharCode = currency.CharCode,
            Nominal = currency.Nominal,
            Name = currency.Name
        };
    }
}