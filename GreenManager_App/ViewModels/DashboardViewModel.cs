using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;

namespace GreenManager_App.ViewModels
{
	public partial class DashboardViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		public DashboardViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		[RelayCommand]
		public async Task NavigateToCustomersAsync()
		{
			// Haal de bestaande klantenpagina op
			var customersPage = _serviceProvider.GetRequiredService<Views.CustomersPage>();

			// PushAsync zorgt ervoor dat we een 'Terug'-knop krijgen in de app!
			var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;
			if (currentPage != null)
			{
				await currentPage.Navigation.PushAsync(customersPage);
			}
		}

		[RelayCommand]
		public void Logout()
		{
			_apiService.Logout();

			MainThread.BeginInvokeOnMainThread(() =>
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window == null) return;

				var loginPage = _serviceProvider.GetRequiredService<Views.LoginPage>();
				window.Page = new NavigationPage(loginPage);
			});
		}
	}
}