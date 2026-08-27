using Models.Entities;
using Models.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GreenManager_App.Services
{
	public class ApiService
	{
		private readonly HttpClient _httpClient;
		private string _jwtToken = string.Empty;
		private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

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
		/// Controleert bij het opstarten van de app of er nog een geldig token is opgeslagen in SecureStorage.
		/// </summary>
		public async Task<bool> InitializeAutoLoginAsync()
		{
			// Haal het token uit de SecureStorage van het toestel
			var opgeslagenToken = await SecureStorage.Default.GetAsync("jwt_token");

			if (!string.IsNullOrEmpty(opgeslagenToken))
			{
				_jwtToken = opgeslagenToken;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _jwtToken);
				return true;
			}
			return false; // Geen token gevonden in SecureStorage
		}

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
		public async Task<List<Customer>> GetCustomersAsync()
			=> await GetAsync<List<Customer>>("api/Customers") ?? new();

		public Task<bool> CreateCustomerAsync(Customer newCustomer)
			=> PostAsync("api/Customers", newCustomer);

		public Task<bool> UpdateCustomerAsync(int id, Customer customer)
			=> PutAsync($"api/Customers/{id}", customer);

		public Task<bool> DeleteCustomerAsync(int id)
			=> DeleteAsync($"api/Customers/{id}");

		// Projects
		public async Task<List<ProjectDto>> GetProjectsAsync()
			=> await GetAsync<List<ProjectDto>>("api/Projects") ?? new();

		public Task<bool> CreateProjectAsync(ProjectRequestDto newProject)
			=> PostAsync("api/Projects", newProject);

		public Task<bool> UpdateProjectAsync(int id, ProjectRequestDto updatedProject)
			=> PutAsync($"api/Projects/{id}", updatedProject);

		public Task<bool> DeleteProjectAsync(int id)
			=> DeleteAsync($"api/Projects/{id}");

		// Materials
		public async Task<List<Material>> GetMaterialsAsync()
			=> await GetAsync<List<Material>>("api/Materials") ?? new();

		public Task<bool> CreateMaterialAsync(Material newMaterial)
			=> PostAsync("api/Materials", newMaterial);

		public Task<bool> UpdateMaterialAsync(int id, Material material)
			=> PutAsync($"api/Materials/{id}", material);

		public Task<bool> DeleteMaterialAsync(int id)
			=> DeleteAsync($"api/Materials/{id}");

		// Employees
		public async Task<List<EmployeeDto>> GetEmployeesAsync()
			=> await GetAsync<List<EmployeeDto>>("api/Employees") ?? new();

		public Task<bool> CreateEmployeeAsync(EmployeeRequestDto newEmployee)
			=> PostAsync("api/Employees", newEmployee);

		public Task<bool> UpdateEmployeeAsync(int id, EmployeeRequestDto employee)
			=> PutAsync($"api/Employees/{id}", employee);

		public Task<bool> DeleteEmployeeAsync(int id)
			=> DeleteAsync($"api/Employees/{id}");

		/// <summary>
		/// Haalt een object of lijst van type T op via GET, met de standaard foutafhandeling
		/// en JWT-controle die voorheen in elke Get...Async-methode herhaald werd.
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
		/// Verstuurt een object via POST en geeft terug of dit gelukt is.
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
		/// Verstuurt een object via PUT en geeft terug of dit gelukt is.
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
		/// Verwijdert een record via DELETE en geeft terug of dit gelukt is.
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
		/// Logt de gebruiker uit door het token te verwijderen.
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