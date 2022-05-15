using DSRProject.Models;

namespace DSRProject.Data.DTO;
public class CourseDTO {
    public uint CourseId { get; set; }
    public float Value { get; set; }
    public DateTime Date { get; set;}
    public Currency Currency { get; set; }
}