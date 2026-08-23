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
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var customersPage = _serviceProvider.GetRequiredService<Views.CustomersPage>();
				await window.Page.Navigation.PushAsync(customersPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToCustomersAsync(): {ex}");
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