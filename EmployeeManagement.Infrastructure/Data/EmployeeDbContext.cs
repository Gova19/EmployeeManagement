using EmployeeManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.JobTitle).IsRequired();
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // ✅ Seed data with constructor usage
                entity.HasData(
                    new
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@ems.com",
                        JobTitle = "Software Engineer",
                        CreatedAt = DateTimeOffset.UtcNow
                    },
                    new
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Jane",
                        LastName = "Smith",
                        Email = "jane.smith@ems.com",
                        JobTitle = "HR Manager",
                        CreatedAt = DateTimeOffset.UtcNow
                    }
                );
            });
        }
    }
}
