using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;

namespace GreenManager_App.ViewModels
{
	public partial class SettingsViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		public SettingsViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		[RelayCommand]
		public async Task LogoutAsync()
		{
			try
			{
				// Verwijder het token via de API service
				_apiService.Logout();

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				// Haal de loginpagina op en overschrijf de root-page, zodat de gebruiker niet terug kan via de 'back' knop
				var loginPage = _serviceProvider.GetRequiredService<Views.LoginPage>();
				window.Page = new NavigationPage(loginPage);

				await Task.CompletedTask;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in LogoutAsync(): {ex}");
			}
		}
	}
}