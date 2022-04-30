using DSRProject.Data;
using DSRProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DSRProject.Pages;

public class DetailsModel : PageModel
{
    private readonly ValuteDbContext _context;
    public IEnumerable<Course> Courses { get; set; } = Enumerable.Empty<Course>();
    public DetailsModel(ValuteDbContext context) => _context = context;
    public async void OnGet() => Courses = await _context.Courses.ToListAsync(); 
}