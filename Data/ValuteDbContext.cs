using DSRProject.Models;
using Microsoft.EntityFrameworkCore;

namespace DSRProject.Data;

public class ValuteDbContext : DbContext {
    public DbSet<Valute> Valutes { get; set; }
    public DbSet<Course> Courses { get; set; }
    public ValuteDbContext(DbContextOptions options) : base(options)
    {
        
    }
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Course>().ToTable("Courses");
    //     modelBuilder.Entity<Valute>().ToTable("Valutes");
    // }
}