using System.ComponentModel.DataAnnotations;

namespace DSRProject.Models;
public class Course {
    [Key]
    public uint CourseId { get; set; }
    public float Value { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set;}
    public Currency Currency { get; set; }
}