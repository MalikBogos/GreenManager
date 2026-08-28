using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;

namespace GreenManager_App.ViewModels
{
	/// <summary>
	/// Beheert de applicatie-instellingen en gebruikersacties waaronder het veilig afmelden van de sessie.
	/// </summary>
	public partial class SettingsViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		public SettingsViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Meldt de huidige gebruiker af door het JWT-token te verwijderen via de ApiService en overschrijft de navigatie-stack zodat de gebruiker terugkeert naar de inlogpagina.
		/// </summary>
		[RelayCommand]
		public async Task LogoutAsync()
		{
			try
			{
				// Verwijdert het token via de API service
				_apiService.Logout();

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				// Haalt de loginpagina op en overschrijft de root-page, zodat de gebruiker niet terug kan via de 'back' knop
				var loginPage = _serviceProvider.GetRequiredService<Views.LoginPage>();
				window.Page = new NavigationPage(loginPage);

				await Task.CompletedTask;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in LogoutAsync(): {ex}");
			}
		}
	}
}