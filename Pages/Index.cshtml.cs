using DSRProject.Data;
using DSRProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DSRProject.Pages;

public class IndexModel : PageModel
{
    private readonly ValuteDbContext _context;
    public IEnumerable<Valute> Valutes { get; set; } = Enumerable.Empty<Valute>();
    public IndexModel(ValuteDbContext context) => _context = context;
    public async void OnGet() => Valutes = await _context.Valutes.ToListAsync();
}
