using Models.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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