using System.ComponentModel.DataAnnotations;

namespace DSRProject.Models;
public class Valute {
    [Key]
    public string ValuteId { get; set; }
    public int NumCode { get; set; }
    public string CharCode { get; set; }
    public int Nominal { get; set; }
    public string Name { get; set; }
}