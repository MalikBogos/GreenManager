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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Project>()
			.Property(p => p.Budget)
			.HasColumnType("decimal(18,2)"); // To indicate the precision of the decimal value inside the table

			var seedDate = new DateTime(2024, 1, 1);

			var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();

			var adminUser = new ApplicationUser
			{
				Id = "admin-uuid-1",
				UserName = "admin@greenmanager.be",
				NormalizedUserName = "ADMIN@GREENMANAGER.BE",
				Email = "admin@greenmanager.be",
				NormalizedEmail = "ADMIN@GREENMANAGER.BE",
				FirstName = "Admin",
				LastName = "User",
				EmailConfirmed = true,
				CreatedAt = seedDate,
				SecurityStamp = "STATIC_STAMP_1",
				ConcurrencyStamp = "STATIC_CONCURRENCY_1"
			};
			adminUser.PasswordHash = hasher.HashPassword(adminUser, "Welkom123!");

			var malikUser = new ApplicationUser
			{
				Id = "user-uuid-2",
				UserName = "malik@greenmanager.be",
				NormalizedUserName = "MALIK@GREENMANAGER.BE",
				Email = "malik@greenmanager.be",
				NormalizedEmail = "MALIK@GREENMANAGER.BE",
				FirstName = "Malik",
				LastName = "Employee",
				EmailConfirmed = true,
				CreatedAt = seedDate,
				SecurityStamp = "STATIC_STAMP_2",
				ConcurrencyStamp = "STATIC_CONCURRENCY_2"
			};

			malikUser.PasswordHash = hasher.HashPassword(malikUser, "Welkom1234!");

			modelBuilder.Entity<ApplicationUser>().HasData(adminUser, malikUser);


			modelBuilder.Entity<Employee>().HasData(
				new Employee
				{
					Id = 1,
					ApplicationUserId = "admin-uuid-1",
					EmployeeNumber = "EMP001",
					HireDate = seedDate,
					JobTitle = "Hoofd Tuinman",
					CreatedAt = seedDate
				},
				new Employee
				{
					Id = 2,
					ApplicationUserId = "user-uuid-2",
					EmployeeNumber = "EMP002",
					HireDate = seedDate,
					JobTitle = "Junior Hovenier",
					CreatedAt = seedDate
				}
			);


			modelBuilder.Entity<Customer>().HasData(
				new Customer
				{
					Id = 1,
					FirstName = "Bob",
					LastName = "Bogos",
					Email = "malik@example.com",
					PhoneNumber = "0488123456",
					Notes = "Eerste testklant",
					CreatedAt = seedDate
				},
				new Customer
				{
					Id = 2,
					FirstName = "Sara",
					LastName = "Groens",
					Email = "sara@test.be",
					PhoneNumber = "0477987654",
					CreatedAt = seedDate
				}
			);

			modelBuilder.Entity<Material>().HasData(
				new Material
				{
					Id = 1,
					Name = "Potgrond Universeel 40L",
					Unit = "Zak",
					PurchasePrice = 6.50m,
					StockQuantity = 100,
					CreatedAt = seedDate
				},
				new Material
				{
					Id = 2,
					Name = "Grasmatten Sport",
					Unit = "m²",
					PurchasePrice = 3.20m,
					StockQuantity = 500,
					CreatedAt = seedDate
				}
			);

			modelBuilder.Entity<Project>().HasData(
				new Project
				{
					Id = 1,
					Name = "Aanleg Stadstuin Antwerpen",
					CustomerId = 1, // Bob
					StartDate = seedDate,
					Status = Models.Enums.ProjectStatus.Active,
					ProjectAddress = "Kerkstraat 1, 2000 Antwerpen",
					CreatedAt = seedDate
				},
				new Project
				{
					Id = 2,
					Name = "Onderhoud Park Gent",
					CustomerId = 2, // Sara
					StartDate = seedDate,
					Status = Models.Enums.ProjectStatus.Pending,
					ProjectAddress = "Veldstraat 10, 9000 Gent",
					CreatedAt = seedDate
				}
			);

			modelBuilder.Entity<ProjectTask>().HasData(
				new ProjectTask
				{
					Id = 1,
					ProjectId = 1,
					Title = "Grondwerken voorbereiden",
					DueDate = seedDate,
					Status = Models.Enums.ProjectTaskStatus.Completed,
					CreatedAt = seedDate
				},
				new ProjectTask
				{
					Id = 2,
					ProjectId = 1,
					Title = "Planten inkopen",
					DueDate = seedDate,
					Status = Models.Enums.ProjectTaskStatus.Active,
					CreatedAt = seedDate
				}
			);
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

				optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
			}
		}

	}
}
