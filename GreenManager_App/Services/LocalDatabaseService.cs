using GreenManager_App.Models.Local;
using SQLite;

namespace GreenManager_App.Services
{
	/// <summary>
	/// Beheert de lokale SQLite-databank op het toestel. 
	/// </summary>
	public class LocalDatabaseService
	{
		private SQLiteAsyncConnection? _database;

		/// <summary>
		/// Maakt (indien nodig) de databankverbinding en alle tabellen aan. Moet voor elk gebruik van deze service aangeroepen worden (bv. bij het opstarten van de app).
		/// </summary>
		public async Task InitializeAsync()
		{
			if (_database != null) return;

			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "greenmanager_local.db3");
			_database = new SQLiteAsyncConnection(dbPath);

			await _database.CreateTableAsync<LocalCustomer>();
			await _database.CreateTableAsync<LocalProject>();
			await _database.CreateTableAsync<LocalMaterial>();
			await _database.CreateTableAsync<LocalEmployee>();
		}

		/// <summary>
		/// Vult de lokale databank met voorbeelddata voor alle vier Models (Models/Local) (enkel voor testdoeleinden).
		/// </summary>
		public async Task SeedAsync()
		{
			try
			{
				if (_database == null) await InitializeAsync();

				await _database!.DeleteAllAsync<LocalCustomer>();
				await _database.DeleteAllAsync<LocalProject>();
				await _database.DeleteAllAsync<LocalMaterial>();
				await _database.DeleteAllAsync<LocalEmployee>();

				var testCustomers = new List<LocalCustomer>
			{
				new()
				{
					ServerId = 1, FirstName = "Jan", LastName = "Janssens",
					Email = "jan.janssens@test.be", PhoneNumber = "0470111111",
					Street = "Dorpsstraat 1", PostalCode = "2000", City = "Antwerpen",
					VATNumber = null, Notes = "Vaste klant, voorkeur voor ochtendafspraken."
				},
				new()
				{
					ServerId = 2, FirstName = "Sofie", LastName = "De Backer", CompanyName = "De Backer Tuinen",
					Email = "info@debackertuinen.be", PhoneNumber = "0470222222",
					Street = "Kerkstraat 5", PostalCode = "9000", City = "Gent",
					VATNumber = "BE0123456789", Notes = "Zakelijke klant, factuur via e-mail."
				},
				new()
				{
					ServerId = 3, FirstName = "Tom", LastName = "Peeters",
					Email = "tom.peeters@test.be", PhoneNumber = "0470333333",
					Street = "Molenweg 12", PostalCode = "3000", City = "Leuven",
					VATNumber = null, Notes = null
				}
			};
				await _database.InsertAllAsync(testCustomers);

				var testProjects = new List<LocalProject>
			{
				new() { ServerId = 1, Name = "Tuinaanleg Janssens", StartDate = DateTime.Today, Status = "InProgress", CustomerId = 1, CustomerLastName = "Janssens", ProjectAddress = "Dorpsstraat 1, 2000 Antwerpen", Budget = 4500 },
				new() { ServerId = 2, Name = "Terrasrenovatie De Backer", StartDate = DateTime.Today.AddDays(5), Status = "Accepted", CustomerId = 2, CustomerLastName = "De Backer", ProjectAddress = "Kerkstraat 5, 9000 Gent", Budget = 3200 }
			};
				await _database.InsertAllAsync(testProjects);

				var testMaterials = new List<LocalMaterial>
			{
				new() { ServerId = 1, Name = "Grastegels", Unit = "stuk", PurchasePrice = 2.50m, StockQuantity = 120 },
				new() { ServerId = 2, Name = "Tuingrond", Unit = "zak", PurchasePrice = 6.95m, StockQuantity = 40 }
			};
				await _database.InsertAllAsync(testMaterials);

				var testEmployees = new List<LocalEmployee>
			{
				new()
				{
					ServerId = 1, EmployeeNumber = "EMP024", FirstName = "Bram", LastName = "Willems",
					JobTitle = "Tuinman", HourlyWage = 18.50m, HireDate = new DateTime(2022, 3, 1),
					DateOfBirth = new DateTime(1994, 7, 12), Street = "Lindenlaan 8", PostalCode = "2600", City = "Berchem",
					Notes = "Beschikbaar op zaterdagen."
				},
				new()
				{
					ServerId = 2, EmployeeNumber = "EMP064", FirstName = "Lotte", LastName = "Van Dam",
					JobTitle = "Ploegleider", HourlyWage = 22.00m, HireDate = new DateTime(2021, 6, 15),
					DateOfBirth = new DateTime(1989, 11, 3), Street = "Stationsplein 4", PostalCode = "9000", City = "Gent",
					Notes = null
				}
			};
				await _database.InsertAllAsync(testEmployees);
			} catch (Exception ex)
			{
				Console.WriteLine($"Fout bij het seeden van de lokale database: {ex.Message}");
			}
		}

		/// <summary>
		/// Haalt alle lokaal opgeslagen klanten op uit de SQLite-databank.
		/// </summary>
		/// <returns>Een lijst van LocalCustomer objecten.</returns>
		public async Task<List<LocalCustomer>> GetCustomersAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalCustomer>().ToListAsync();
		}

		/// <summary>
		/// Haalt alle lokaal opgeslagen projecten op uit de SQLite-databank.
		/// </summary>
		/// <returns>Een lijst van LocalProject objecten.</returns>
		public async Task<List<LocalProject>> GetProjectsAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalProject>().ToListAsync();
		}

		/// <summary>
		/// Haalt al het lokaal opgeslagen materiaal op uit de SQLite-databank.
		/// </summary>
		/// <returns>Een lijst van LocalMaterial objecten.</returns>
		public async Task<List<LocalMaterial>> GetMaterialsAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalMaterial>().ToListAsync();
		}

		/// <summary>
		/// Haalt alle lokaal opgeslagen werknemers op uit de SQLite-databank.
		/// </summary>
		/// <returns>Een lijst van LocalEmployee objecten.</returns>
		public async Task<List<LocalEmployee>> GetEmployeesAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalEmployee>().ToListAsync();
		}
	}
}