using System.ComponentModel.DataAnnotations;

namespace DSRProject.Models;
public class Currency {
    [Key]
    public string CurrencyId { get; set; }
    public int NumCode { get; set; }
    public string CharCode { get; set; }
    public int Nominal { get; set; }
    public string Name { get; set; }
}