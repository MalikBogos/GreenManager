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
		/// Maakt (indien nodig) de databankverbinding en alle tabellen aan. Moet voor elk ander
		/// gebruik van deze service aangeroepen worden (bv. bij het opstarten van de app).
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
		/// Vult de lokale databank met voorbeelddata voor alle vier de entiteiten, enkel voor
		/// testdoeleinden.
		/// </summary>
		public async Task SeedAsync()
		{
			if (_database == null) await InitializeAsync();

			await _database!.DeleteAllAsync<LocalCustomer>();
			await _database.DeleteAllAsync<LocalProject>();
			await _database.DeleteAllAsync<LocalMaterial>();
			await _database.DeleteAllAsync<LocalEmployee>();

			var testCustomers = new List<LocalCustomer>
			{
				new() { ServerId = 1, FirstName = "Jan", LastName = "Janssens", Email = "jan.janssens@test.be", PhoneNumber = "0470111111" },
				new() { ServerId = 2, FirstName = "Sofie", LastName = "De Backer", CompanyName = "De Backer Tuinen", Email = "info@debackertuinen.be", PhoneNumber = "0470222222" },
				new() { ServerId = 3, FirstName = "Tom", LastName = "Peeters", Email = "tom.peeters@test.be", PhoneNumber = "0470333333" }
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
				new() { ServerId = 1, EmployeeNumber = "EMP024", FirstName = "Bram", LastName = "Willems", JobTitle = "Tuinman", HourlyWage = 18.50m, HireDate = new DateTime(2022, 3, 1) },
				new() { ServerId = 2, EmployeeNumber = "EMP064", FirstName = "Lotte", LastName = "Van Dam", JobTitle = "Ploegleider", HourlyWage = 22.00m, HireDate = new DateTime(2021, 6, 15) }
			};
			await _database.InsertAllAsync(testEmployees);
		}

		public async Task<List<LocalCustomer>> GetCustomersAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalCustomer>().ToListAsync();
		}

		public async Task<List<LocalProject>> GetProjectsAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalProject>().ToListAsync();
		}

		public async Task<List<LocalMaterial>> GetMaterialsAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalMaterial>().ToListAsync();
		}

		public async Task<List<LocalEmployee>> GetEmployeesAsync()
		{
			if (_database == null) await InitializeAsync();
			return await _database!.Table<LocalEmployee>().ToListAsync();
		}
	}
}