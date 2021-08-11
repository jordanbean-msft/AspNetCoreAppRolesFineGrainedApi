using AspNetCoreWithAppRolesAndFineGrained.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWithAppRolesAndFineGrained.Data {
  public class AspNetCoreWithAppRolesAndFineGrainedDbContext : DbContext {
    public AspNetCoreWithAppRolesAndFineGrainedDbContext(DbContextOptions<AspNetCoreWithAppRolesAndFineGrainedDbContext> options) : base(options) {

    }

    public DbSet<Branch> Branches { get; set; }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Salary> Salaries { get; set; }

    public DbSet<Sale> Sales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<Branch>().ToTable("Branch");
      modelBuilder.Entity<Employee>().ToTable("Employee");
      modelBuilder.Entity<Salary>().ToTable("Salary");
      modelBuilder.Entity<Sale>().ToTable("Sale");
    }
  }
}