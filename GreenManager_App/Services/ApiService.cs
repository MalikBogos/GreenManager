using Models.Entities;
using Models.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GreenManager_App.Services
{
	/// <summary>
	/// Zorgt voor alle communicatie tussen het MAUI project (GreenManager_App) en het ASP.NET CORE MVC project (GreenManager_Web), beheert ook JWT tokens voor de (automatische) authenticatie en bezit de logica voor CRUD-operaties op Customers, Projects, Materials en Employees. Generieken werden gebruikt om herhaling in de methodes te verminderen.
	/// </summary>
	public class ApiService
	{
		private readonly HttpClient _httpClient;
		private string _jwtToken = string.Empty;
		private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

		/// <summary>
		/// Stelt de HttpClient in afhankelijk van het platform waarop wordt gewerkt. Tijdens ontwikkeling wordt voor Android aangepaste certificaatvalidatie gebruikt.
		/// </summary>
		public ApiService()
		{
#if DEBUG
			var handler = new HttpsClientHandlerService();
			_httpClient = new HttpClient(handler.GetPlatformMessageHandler());
#else
			_httpClient = new HttpClient();
#endif
			// Controleer op welk platform de app draait en stel de juiste URL in
			
			string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
				? "https://10.0.2.2:7086/"
				: "https://localhost:7086/";

			_httpClient.BaseAddress = new Uri(baseUrl);
		}


		/// <summary>
		/// Controleert bij het opstarten van de MAUI-app of er nog een geldig JWT-token is opgeslagen in SecureStorage.
		/// </summary>
		/// <returns>True indien er een geldig JWT-token aanwezig is, anders false.</returns>
		public async Task<bool> InitializeAutoLoginAsync()
		{
			// Haal het token uit de SecureStorage van het toestel
			var savedToken = await SecureStorage.Default.GetAsync("jwt_token");

			if (!string.IsNullOrEmpty(savedToken))
			{
				_jwtToken = savedToken;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _jwtToken);
				return true;
			}
			return false; // Geen token gevonden in SecureStorage
		}

		/// <summary>
		/// Meldt een gebruiker aan via de API met Email, Password en RememberMe. Het JWT-token wordt ingesteld als authorization-header en bewaard in de SecureStorage.
		/// </summary>
		/// <param name="email">Verwijst naar het emailadres waarmee de gebruiker zich aanmeldt.</param>
		/// <param name="password">Verwijst naar het wachtwoord waarmee de gebruiker met zijn emailadres aanmeldt.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public async Task<bool> LoginAsync(string email, string password)
		{
			try
			{
				var loginData = new { Email = email, Password = password, RememberMe = true };
				var response = await _httpClient.PostAsJsonAsync("api/authentication/login", loginData);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsStringAsync();
					using var doc = JsonDocument.Parse(result);
					_jwtToken = doc.RootElement.GetProperty("token").GetString() ?? "";

					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _jwtToken);

                    // Sla het token op voor de volgende inlog
					await SecureStorage.Default.SetAsync("jwt_token", _jwtToken);
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		// Customers
		/// <summary>
		/// Haalt alle klanten op via de Customers API.
		/// </summary>
		/// <returns>Een lijst van klanten (Customers) of een lege lijst indien er een fout is opgetreden.</returns>
		public async Task<List<Customer>> GetCustomersAsync()
			=> await GetAsync<List<Customer>>("api/Customers") ?? new();

		/// <summary>
		/// Maakt een nieuwe klant aan via de API.
		/// </summary>
		/// <param name="newCustomer">De klantgegevens om aan te maken.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> CreateCustomerAsync(Customer newCustomer)
			=> PostAsync("api/Customers", newCustomer);

		/// <summary>
		/// Werkt een bestaande klant bij via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van de klant die bijgewerkt wordt.</param>
		/// <param name="customer">De bijgewerkte klantgegevens.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> UpdateCustomerAsync(int id, Customer customer)
			=> PutAsync($"api/Customers/{id}", customer);

		/// <summary>
		/// Verwijdert (soft-delete) een klant via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van de klant die verwijderd wordt.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> DeleteCustomerAsync(int id)
			=> DeleteAsync($"api/Customers/{id}");

		// Projects
		/// <summary>
		/// Haalt alle projecten op via de Project API als DTO.
		/// </summary>
		/// <returns>Een lijst van ProjectDto of een lege lijst indien er een fout is opgetreden.</returns>
		public async Task<List<ProjectDto>> GetProjectsAsync()
			=> await GetAsync<List<ProjectDto>>("api/Projects") ?? new();

		/// <summary>
		/// Maakt een nieuw project aan via de API.
		/// </summary>
		/// <param name="newProject">De projectgegevens om aan te maken.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> CreateProjectAsync(ProjectRequestDto newProject)
			=> PostAsync("api/Projects", newProject);

		/// <summary>
		/// Werkt een bestaand project bij via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van het project dat bijgewerkt wordt.</param>
		/// <param name="updatedProject">De bijgewerkte projectgegevens.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> UpdateProjectAsync(int id, ProjectRequestDto updatedProject)
			=> PutAsync($"api/Projects/{id}", updatedProject);

		/// <summary>
		/// Verwijdert (soft-delete) een project via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van het project dat verwijderd wordt.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> DeleteProjectAsync(int id)
			=> DeleteAsync($"api/Projects/{id}");

		// Materials
		/// <summary>
		/// Haalt al het materiaal op via de API.
		/// </summary>
		/// <returns>Een lijst van materiaal (Materials) of een lege lijst indien er een fout is opgetreden.</returns>
		public async Task<List<Material>> GetMaterialsAsync()
			=> await GetAsync<List<Material>>("api/Materials") ?? new();

		/// <summary>
		/// Maakt een nieuw materiaal via de API.
		/// </summary>
		/// <param name="newMaterial">De materiaalgegevens om aan te maken.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> CreateMaterialAsync(Material newMaterial)
			=> PostAsync("api/Materials", newMaterial);

		/// <summary>
		/// Werkt een bestaand materiaal bij via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van het materiaal dat bijgewerkt wordt.</param>
		/// <param name="material">De bijgewerkte materiaalgegevens.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> UpdateMaterialAsync(int id, Material material)
			=> PutAsync($"api/Materials/{id}", material);

		/// <summary>
		/// Verwijdert (soft-delete) een materiaal via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van het materiaal dat verwijderd wordt.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> DeleteMaterialAsync(int id)
			=> DeleteAsync($"api/Materials/{id}");

		// Employees
		/// <summary>
		/// Haalt alle werknemers op via de Employees API als DTO.
		/// </summary>
		/// <returns>Een lijst van EmployeeDto of een lege lijst indien er een fout is opgetreden.</returns>
		public async Task<List<EmployeeDto>> GetEmployeesAsync()
			=> await GetAsync<List<EmployeeDto>>("api/Employees") ?? new();

		/// <summary>
		/// Maakt een nieuwe werknemer aan via de API.
		/// </summary>
		/// <param name="newEmployee">De werknemergegevens om aan te maken.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> CreateEmployeeAsync(EmployeeRequestDto newEmployee)
			=> PostAsync("api/Employees", newEmployee);

		/// <summary>
		/// Werkt een bestaande werknemer bij via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van de werknemer die bijgewerkt wordt.</param>
		/// <param name="employee">De bijgewerkte werknemergegevens.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> UpdateEmployeeAsync(int id, EmployeeRequestDto employee)
			=> PutAsync($"api/Employees/{id}", employee);

		/// <summary>
		/// Verwijdert (soft-delete) een werknemer via de API.
		/// </summary>
		/// <param name="id">Verwijst naar het Id van de werknemer die verwijderd wordt.</param>
		/// <returns>True indien geslaagd, anders false.</returns>
		public Task<bool> DeleteEmployeeAsync(int id)
			=> DeleteAsync($"api/Employees/{id}");

		/// <summary>
		/// Haalt een object of lijst van type T op via GET met de standaard foutafhandeling (trycatch) en doet een JWT-token controle.
		/// </summary>
		private async Task<T?> GetAsync<T>(string endpoint)
		{
			if (string.IsNullOrEmpty(_jwtToken)) return default;

			try
			{
				var response = await _httpClient.GetAsync(endpoint);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij GET {endpoint}: {ex.Message}");
			}

			return default;
		}

		/// <summary>
		/// Verstuurt een object via POST en geeft terug of dit is gelukt.
		/// </summary>
		private async Task<bool> PostAsync<T>(string endpoint, T payload)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij POST {endpoint}: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Verstuurt een object via PUT en geeft terug of dit is gelukt.
		/// </summary>
		private async Task<bool> PutAsync<T>(string endpoint, T payload)
		{
			try
			{
				var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij PUT {endpoint}: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Verwijdert een entry in de database via DELETE en geeft terug of dit is gelukt.
		/// </summary>
		private async Task<bool> DeleteAsync(string endpoint)
		{
			try
			{
				var response = await _httpClient.DeleteAsync(endpoint);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij DELETE {endpoint}: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Meldt de gebruiker af door het JWT-token te verwijderen uit de SecureStorage en Authorization-header.
		/// </summary>
		public void Logout()
		{
			// Verwijder het token
			_jwtToken = string.Empty;
			_httpClient.DefaultRequestHeaders.Authorization = null;

			// Verwijder het token uit de SecureStorage zodat de autologin de volgende keer faalt
			SecureStorage.Default.Remove("jwt_token");
		}
	}
}