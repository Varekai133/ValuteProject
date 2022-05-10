using DSRProject.Models;
using Microsoft.EntityFrameworkCore;

namespace DSRProject.Data;

public class CurrencyDbContext : DbContext {
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Course> Courses { get; set; }
    public CurrencyDbContext(DbContextOptions options) : base(options)
    {
        
    }
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Course>().ToTable("Courses");
    //     modelBuilder.Entity<Valute>().ToTable("Valutes");
    // }
}