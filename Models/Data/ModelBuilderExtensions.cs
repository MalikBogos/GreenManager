using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Data
{
	public static class ModelBuilderExtensions
	{
		public static void Seed(this ModelBuilder modelBuilder)
		{
			var seedDate = new DateTime(2024, 1, 1);
			var dateOfBirth = new DateTime(1999, 2, 1);
			var startDate = new DateTime(2026, 09, 01);
			var endDate = new DateTime(2026, 09, 25);

			// 1. ROLLEN AANMAKEN
			var adminRole = new IdentityRole { Id = "role-admin-1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "STATIC_ROLE_CONCURRENCY_1"
			};
			var employeeRole = new IdentityRole { Id = "role-employee-2", Name = "Employee", NormalizedName = "EMPLOYEE", ConcurrencyStamp = "STATIC_ROLE_CONCURRENCY_2"
			};
			var guestRole = new IdentityRole { Id = "role-guest-3", Name = "Guest", NormalizedName = "GUEST", ConcurrencyStamp = "STATIC_ROLE_CONCURRENCY_3"
			};

			modelBuilder.Entity<IdentityRole>().HasData(adminRole, employeeRole, guestRole);

			// 2. INLOGACCOUNTS (USERS) AANMAKEN

			var adminUser = new ApplicationUser
			{
				Id = "admin-uuid-1",
				UserName = "admin@greenmanager.be",
				NormalizedUserName = "ADMIN@GREENMANAGER.BE",
				Email = "admin@greenmanager.be",
				NormalizedEmail = "ADMIN@GREENMANAGER.BE",
				FirstName = "Jan",
				LastName = "Vandekapelle",
				PhoneNumber = "0485760623",
				PhoneNumberConfirmed = false,
				EmailConfirmed = false,
				CreatedAt = seedDate,
				SecurityStamp = "STATIC_STAMP_1",
				ConcurrencyStamp = "STATIC_CONCURRENCY_1",
				PasswordHash = "AQAAAAIAAYagAAAAEDBlDV8N8fYe0zL1l6fo716II8lI0fh/IP++gps1UZ79fOPsPtk9S72PukWm4Oh1sA=="
			};
			adminUser.PasswordHash = "AQAAAAIAAYagAAAAEDBlDV8N8fYe0zL1l6fo716II8lI0fh/IP++gps1UZ79fOPsPtk9S72PukWm4Oh1sA==";

			var employeeUser = new ApplicationUser
			{
				Id = "employee-uuid-2",
				UserName = "employee@greenmanager.be",
				NormalizedUserName = "EMPLOYEE@GREENMANAGER.BE",
				Email = "employee@greenmanager.be",
				NormalizedEmail = "EMPLOYEE@GREENMANAGER.BE",
				FirstName = "John",
				LastName = "Doe",
				PhoneNumber = "0485767312",
				PhoneNumberConfirmed = false,
				EmailConfirmed = true,
				CreatedAt = seedDate,
				SecurityStamp = "STATIC_STAMP_2",
				ConcurrencyStamp = "STATIC_CONCURRENCY_2",
				PasswordHash = "AQAAAAIAAYagAAAAEDBlDV8N8fYe0zL1l6fo716II8lI0fh/IP++gps1UZ79fOPsPtk9S72PukWm4Oh1sA=="
			};
			employeeUser.PasswordHash = "AQAAAAIAAYagAAAAEDBlDV8N8fYe0zL1l6fo716II8lI0fh/IP++gps1UZ79fOPsPtk9S72PukWm4Oh1sA==";

			var guestUser = new ApplicationUser
			{
				Id = "guest-uuid-3",
				UserName = "guest@greenmanager.be",
				NormalizedUserName = "GUEST@GREENMANAGER.BE",
				Email = "guest@greenmanager.be",
				NormalizedEmail = "GUEST@GREENMANAGER.BE",
				FirstName = "Gaston",
				LastName = "Verwelkomd",
				EmailConfirmed = false,
				CreatedAt = seedDate,
				SecurityStamp = "STATIC_STAMP_3",
				ConcurrencyStamp = "STATIC_CONCURRENCY_3",
				PasswordHash = "AQAAAAIAAYagAAAAEDBlDV8N8fYe0zL1l6fo716II8lI0fh/IP++gps1UZ79fOPsPtk9S72PukWm4Oh1sA=="
			};
			guestUser.PasswordHash = "AQAAAAIAAYagAAAAEDBlDV8N8fYe0zL1l6fo716II8lI0fh/IP++gps1UZ79fOPsPtk9S72PukWm4Oh1sA==";

			modelBuilder.Entity<ApplicationUser>().HasData(adminUser, employeeUser, guestUser);

			// 3. KOPPELING TUSSEN USERS EN ROLLEN
			modelBuilder.Entity<IdentityUserRole<string>>().HasData(
				new IdentityUserRole<string> { RoleId = "role-admin-1", UserId = "admin-uuid-1" },
				new IdentityUserRole<string> { RoleId = "role-employee-2", UserId = "employee-uuid-2" },
				new IdentityUserRole<string> { RoleId = "role-guest-3", UserId = "guest-uuid-3" }
			);

			// 4. WERKNEMERSDOSSIERS
			modelBuilder.Entity<Employee>().HasData(
				new Employee { Id = 1, ApplicationUserId = "admin-uuid-1", EmployeeNumber = "EMP001", HourlyWage = 35.00m, HireDate = seedDate, DateOfBirth = dateOfBirth, JobTitle = "Hoofd Tuinman", Street = "Nijverheidskaai 138", PostalCode = "1070", City = "Anderlecht", Notes = "Eigenaar van het bedrijf", CreatedAt = seedDate },

				new Employee { Id = 2, ApplicationUserId = "employee-uuid-2", EmployeeNumber = "EMP002", HourlyWage = 22.50m, HireDate = seedDate, DateOfBirth = dateOfBirth, JobTitle = "Junior Tuinman", Street = "Nijverheidskaai 138", PostalCode = "1070", City = "Anderlecht", Notes = "Eerste medewerker van het bedrijf", CreatedAt = seedDate }
			);

			// 5. KLANTEN
			modelBuilder.Entity<Customer>().HasData(
				new Customer { Id = 1, FirstName = "Bob", LastName = "Vandestraat", CompanyName = "Apple", VATNumber = "5435882443", Notes = "Eerste testklant", Email = "bob@example.com", PhoneNumber = "0488123456", Street = "Bergensesteenweg 322", PostalCode = "1600", City = "Sint-Pieters-Leeuw", CreatedAt = seedDate },
				new Customer { Id = 2, FirstName = "Sara", LastName = "Groens",CompanyName = "Microsoft", VATNumber = "347238473", Notes = "Tweede testklant", Email = "sara@test.be", PhoneNumber = "0477987654", Street = "Brusselsesteenweg 10", PostalCode = "1500", City = "Halle", CreatedAt = seedDate }
			);

			// 6. MATERIALEN
			modelBuilder.Entity<Material>().HasData(
				new Material { Id = 1, Name = "Potgrond Universeel 40L", Description = "40L potgrond voor de grond", Unit = "Zak", PurchasePrice = 6.50m, StockQuantity = 100, Notes = "Gebruikt voor gaten in de grond", CreatedAt = seedDate },

				new Material { Id = 2, Name = "Grasmatten Sport", Description = "Grasmatten gebruikt voor fitness", Unit = "m2", PurchasePrice = 3.20m, StockQuantity = 500, Notes = "Weinig wrijving dus goed voor sprint", CreatedAt = seedDate }
			);

			// 7. PROJECTEN
			modelBuilder.Entity<Project>().HasData(
				new Project { Id = 1, Name = "Aanleg Stadstuin Antwerpen", Description = "Ons eerste project", StartDate = startDate, EndDate = endDate, Status = Models.Enums.ProjectStatus.Accepted, CustomerId = 1, ProjectAddress = "Kerkstraat 1, 2000 Antwerpen", Budget = 3500.00m, Notes = "Deze klant is zeer belangrijk", CreatedAt = seedDate },

				new Project { Id = 2, Name = "Onderhoud Park Gent", Description = "Ons tweede project", StartDate = startDate, EndDate = endDate, Status = Models.Enums.ProjectStatus.Quotation, CustomerId = 2, ProjectAddress = "Veldstraat 10, 9000 Gent", Budget = 4000.00m, Notes = "Moeilijk toegankelijke werkplaats", CreatedAt = seedDate }
			);

			// 8. PROJECT PLANNING
			modelBuilder.Entity<ProjectEmployee>().HasData(
				new ProjectEmployee { Id = 1, ProjectId = 1, EmployeeId = 2, PlannedDate = startDate.AddDays(10), EstimatedHours = 8m, Notes = "Niet elke dag beschikbaar voor deze job", CreatedAt = seedDate }
			);

			// 9. PROJECT MATERIALEN
			modelBuilder.Entity<ProjectMaterial>().HasData(
				new ProjectMaterial { Id = 1, Quantity = 5m, ProjectId = 1, MaterialId = 1, CreatedAt = seedDate }
			);

			// 10. WERKUREN REGISTRATIE
			modelBuilder.Entity<WorkLog>().HasData(
				new WorkLog { Id = 1, EmployeeId = 2, ProjectId = 1,  WorkDate = startDate.AddDays(10), TaskDescription = "Aanleg bomen", HoursWorked = 7.5m, Notes = "Proefperiode", HourlyWageAtTime = 22.50m, CreatedAt = seedDate }
			);
		}
	}
}
