using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;

namespace GreenManager_App.ViewModels
{
	/// <summary>
	/// ViewModel voor de LoginPage (MVVM). Beheert de aanmelding en authenticatie voor de MAUI app. Communiceert met de ApiService voor de aanmelding en het aanmaken van het JWT-token.
	/// </summary>
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Het ingevoerde emailadres, gebonden aan het invoerveld in de UI met Bindings.
		/// </summary>
		[ObservableProperty]
		public partial string Email { get; set; } = string.Empty;

		/// <summary>
		/// Het ingevoerde wachtwoord, gebonden aan het invoerveld in de UI met Bindings.
		/// </summary>
		[ObservableProperty]
		public partial string Password { get; set; } = string.Empty;

		/// <summary>
		/// De foutmelding die getoond wordt als de aanmelding mislukt of de invoer ongeldig is.
		/// </summary>
		[ObservableProperty]
		public partial string ErrorMessage { get; set; } = string.Empty;

		/// <summary>
		/// Geeft aan of er momenteel een foutmelding getoond moet worden in de UI.
		/// </summary>
		[ObservableProperty]
		public partial bool HasError { get; set; }


        public LoginViewModel(ApiService apiService, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _serviceProvider = serviceProvider;
        }

		/// <summary>
		/// Valideert het emailadres en wachtwoord en probeert vervolgens aan te melden via de API. Navigeert naar DashboardPage bij succesvolle aanmelding en toont anders een foutmelding.
		/// </summary>
		[RelayCommand]
		public async Task LoginAsync()
		{
			HasError = false;
			ErrorMessage = string.Empty;

			if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
			{
				ErrorMessage = "Vul e-mail en wachtwoord in.";
				HasError = true;
				return;
			}

			// Probeer in te loggen via de API (gebeurt op de achtergrond)
			bool isSuccess = await _apiService.LoginAsync(Email, Password);

			try
			{
				if (isSuccess)
				{
					// Roep de Main Thread aan om het scherm veilig te wisselen
					MainThread.BeginInvokeOnMainThread(() =>
					{
						var window = Application.Current?.Windows.FirstOrDefault();
						if (window == null) return;

						var dashboardPage = _serviceProvider.GetRequiredService<Views.DashboardPage>();

						window.Page = new NavigationPage(dashboardPage);
					});
				}
				else
				{
					ErrorMessage = "Inloggen mislukt. Controleer je gegevens of accountstatus.";
					HasError = true;
				}
			} catch (Exception ex)
			{
				Console.WriteLine($"Fout in LoginAsync(): {ex}");
			}
			
		}
	}
}