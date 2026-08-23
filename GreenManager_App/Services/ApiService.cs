using Models.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Models.DTOs;

namespace GreenManager_App.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _jwtToken = string.Empty;

        public ApiService()
        {
#if DEBUG
			var handler = new HttpsClientHandlerService();
			_httpClient = new HttpClient(handler.GetPlatformMessageHandler());
#else
            _httpClient = new HttpClient();
#endif

			// 10.0.2.2 is de manier waarop de Android Emulator met het https project communiceert
			_httpClient.BaseAddress = new Uri("https://10.0.2.2:7086/"); 
        }

		/// <summary>
		/// Controleert bij het opstarten van de app of er nog een geldig token is opgeslagen in SecureStorage
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
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/authentication/login", content);

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

		// ---------------------------- Customers

		/// <summary>
		/// Haalt de actieve klanten op via de API, met gebruik van het JWT-token.
		/// </summary>
		public async Task<List<Customer>> GetCustomersAsync()
		{
			// Toon een lege lijst indien het JWT token ongeldig is
			if (string.IsNullOrEmpty(_jwtToken))
			{
				return new List<Customer>();
			}

			try
			{
				// Stuur een verzoek naar /Api/Customers
				var response = await _httpClient.GetAsync("api/Customers");

				if (response.IsSuccessStatusCode)
				{
					string jsonResult = await response.Content.ReadAsStringAsync();

					// Zet de JSON tekst om in een C# Lijst van Customers
					var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
					var customers = JsonSerializer.Deserialize<List<Customer>>(jsonResult, options);

					// Geef de lijst terug. Als 'customers' null is, geef dan een nieuwe lege lijst terug.
					return customers ?? new List<Customer>();
				}
			}
			catch (Exception ex)
			{
				// Als het ophalen van gegevens faalt, tonen we de foutmelding in de console ipv. een applicatiecrash
				Console.WriteLine($"Fout in GetCustomersAsync(): {ex.Message}");
			}

			// Geef een lege lijst terug indien er iets mis ging
			return new List<Customer>();
		}

		/// <summary>
		/// Stuurt een nieuwe klant naar de API om opgeslagen te worden
		/// </summary>
		public async Task<bool> CreateCustomerAsync(Customer newCustomer)
		{
			try
			{
				// Zet het C# Klant object om naar JSON-tekst
				var json = JsonSerializer.Serialize(newCustomer);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				// Stuur een POST verzoek naar de ASP.NET API
				var response = await _httpClient.PostAsync("api/Customers", content);

				// Geeft True terug als de server succesvol antwoordt
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij toevoegen van klant: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> UpdateCustomerAsync(int id, Customer customer)
		{
			try
			{
				var json = JsonSerializer.Serialize(customer);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PutAsync($"api/Customers/{id}", content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in UpdateCustomerAsync: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> DeleteCustomerAsync(int id)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"api/Customers/{id}");
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in DeleteCustomerAsync: {ex.Message}");
				return false;
			}
		}



		// ------------------------------ Projects

		public async Task<List<ProjectDto>> GetProjectsAsync()
		{
			try
			{
				if (string.IsNullOrEmpty(_jwtToken)) return new List<ProjectDto>();

				var response = await _httpClient.GetAsync("api/Projects");
				if (response.IsSuccessStatusCode)
				{
					string jsonResult = await response.Content.ReadAsStringAsync();
					var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
					var projects = JsonSerializer.Deserialize<List<ProjectDto>>(jsonResult, options);
					return projects ?? new List<ProjectDto>();
				}
				return new List<ProjectDto>();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetProjectsAsync: {ex}");
				return new List<ProjectDto>();
			}
		}

		public async Task<bool> CreateProjectAsync(ProjectRequestDto newProject)
		{
			try
			{
				var json = JsonSerializer.Serialize(newProject);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync("api/Projects", content);

				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in CreateProjectAsync: {ex}");
				return false;
			}
		}

		public async Task<bool> UpdateProjectAsync(int id, ProjectRequestDto updatedProject)
		{
			try
			{
				var json = JsonSerializer.Serialize(updatedProject);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PutAsync($"api/Projects/{id}", content);

				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in UpdateProjectAsync: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> DeleteProjectAsync(int id)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"api/Projects/{id}");
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in DeleteProjectAsync: {ex.Message}");
				return false;
			}
		}




		// ------------------------------ Materials


		public async Task<List<Material>> GetMaterialsAsync()
		{
			try
			{
				if (string.IsNullOrEmpty(_jwtToken)) return new List<Material>();

				var response = await _httpClient.GetAsync("api/Materials");
				if (response.IsSuccessStatusCode)
				{
					string jsonResult = await response.Content.ReadAsStringAsync();
					var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
					var materials = JsonSerializer.Deserialize<List<Material>>(jsonResult, options);
					return materials ?? new List<Material>();
				}
				return new List<Material>();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetMaterialsAsync: {ex.Message}");
				return new List<Material>();
			}
		}

		public async Task<bool> CreateMaterialAsync(Material newMaterial)
		{
			try
			{
				var json = JsonSerializer.Serialize(newMaterial);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync("api/Materials", content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in CreateMaterialAsync: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> UpdateMaterialAsync(int id, Material material)
		{
			try
			{
				var json = JsonSerializer.Serialize(material);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PutAsync($"api/Materials/{id}", content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in UpdateMaterialAsync: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> DeleteMaterialAsync(int id)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"api/Materials/{id}");
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in DeleteMaterialAsync: {ex.Message}");
				return false;
			}
		}


		// ------------------------------- Employees

		public async Task<List<EmployeeDto>> GetEmployeesAsync()
		{
			try
			{
				if (string.IsNullOrEmpty(_jwtToken)) return new List<EmployeeDto>();

				var response = await _httpClient.GetAsync("api/Employees");
				if (response.IsSuccessStatusCode)
				{
					string jsonResult = await response.Content.ReadAsStringAsync();
					var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
					var employees = JsonSerializer.Deserialize<List<EmployeeDto>>(jsonResult, options);
					return employees ?? new List<EmployeeDto>();
				}
				return new List<EmployeeDto>();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetEmployeesAsync: {ex}");
				return new List<EmployeeDto>();
			}
		}

		public async Task<bool> CreateEmployeeAsync(EmployeeRequestDto newEmployee)
		{
			try
			{
				var json = JsonSerializer.Serialize(newEmployee);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync("api/Employees", content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in CreateEmployeeAsync: {ex}");
				return false;
			}
		}

		public async Task<bool> UpdateEmployeeAsync(int id, EmployeeRequestDto employee)
		{
			try
			{
				var json = JsonSerializer.Serialize(employee);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PutAsync($"api/Employees/{id}", content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in UpdateEmployeeAsync: {ex}");
				return false;
			}
		}

		public async Task<bool> DeleteEmployeeAsync(int id)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"api/Employees/{id}");
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in DeleteEmployeeAsync: {ex}");
				return false;
			}
		}

		/// <summary>
		/// Logt de gebruiker uit door het token te verwijderen
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