using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Data
{
	public class GreenManagerDbContext : IdentityDbContext<ApplicationUser>
	{
		public GreenManagerDbContext(DbContextOptions<GreenManagerDbContext> options) : base(options) 
		{

		}

		public DbSet<Address> Addresses { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<EmployeeWageHistory> EmployeeWageHistories { get; set; }
		public DbSet<Material> Materials { get; set; }
		public DbSet<MaterialCategory> MaterialCategories { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<ProjectEmployee> ProjectEmployees { get; set; }
		public DbSet<ProjectMaterial> ProjectMaterials { get; set; }
		public DbSet<ProjectTask> ProjectTasks { get; set; }
		public DbSet<Quote> Quotes { get; set; }
		public DbSet<QuoteItem> QuoteItems { get; set; }
		public DbSet<WorkLog> WorkLogs { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;IntegratedSecurity=True;");
		}


	}
}
