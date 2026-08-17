using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Models.Data
{
	public class GreenManagerDbContext : IdentityDbContext<ApplicationUser>
	{

		public GreenManagerDbContext() { }

		public GreenManagerDbContext(DbContextOptions<GreenManagerDbContext> options) : base(options)
		{

		}

		public DbSet<Customer> Customers { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Material> Materials { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<ProjectEmployee> ProjectEmployees { get; set; }
		public DbSet<ProjectMaterial> ProjectMaterials { get; set; }
		public DbSet<WorkLog> WorkLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Project>()
			.Property(p => p.Budget)
			.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<Employee>()
			.HasOne(e => e.User)
			.WithOne(u => u.Employee)
			.HasForeignKey<Employee>(e => e.ApplicationUserId)
			.IsRequired();

			modelBuilder.Seed();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{

			if (!optionsBuilder.IsConfigured)
			{
				IConfigurationRoot configuration = new ConfigurationBuilder()
					.AddUserSecrets<GreenManagerDbContext>()
					.Build();

				var connectionString = configuration.GetConnectionString("DefaultConnection");

				optionsBuilder.UseSqlServer(connectionString);

				//optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
			}
		}
	}
}