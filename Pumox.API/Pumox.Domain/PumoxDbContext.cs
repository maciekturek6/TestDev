using Microsoft.EntityFrameworkCore;
using Pumox.Domain.Entities;
using Pumox.Domain.Enums;
using System;

namespace Pumox.Domain
{
    public class PumoxDbContext : DbContext
    {
        public PumoxDbContext(DbContextOptions<PumoxDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Company>()
            //    .Property(p => p.CompanyId)
            //    .HasColumnType("long");

            //modelBuilder.Entity<Employee>()
            //    .Property(p => p.EmployeeId)
            //    .HasColumnType("long");
        }
    }
}
