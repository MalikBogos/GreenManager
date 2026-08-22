using GreenManager_App.Services;
using GreenManager_App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GreenManager_App
{
	public partial class App : Application
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ApiService _apiService;

		public App(IServiceProvider serviceProvider, ApiService apiService)
		{
			InitializeComponent();
			_serviceProvider = serviceProvider;
			_apiService = apiService;
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			// We starten tijdelijk op met een leeg laadscherm tijdens het wachten op de CheckAutoLoginAsync()
			var window = new Window (new ContentPage { Content = new ActivityIndicator { IsRunning = true, VerticalOptions = LayoutOptions.Center } });

			// Controleer auto login op de achtergrond
			CheckAutoLoginAsync(window);

			return window;
		}

		private async void CheckAutoLoginAsync(Window window)
		{
			try
			{
				// Checkt of er een JWT token in de SecureStorage zit
				bool isIngelogd = await _apiService.InitializeAutoLoginAsync();

				MainThread.BeginInvokeOnMainThread(() =>
				{
					if (isIngelogd)
					{
						// Opent automatisch dashboardPage indien het token aanwezig en geldig is
						var dashboardPage = _serviceProvider.GetRequiredService<DashboardPage>();
						window.Page = new NavigationPage(dashboardPage);
					}
					else
					{
						// Opent automatisch LoginPage indien het token niet geldig is 
						var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
						window.Page = new NavigationPage(loginPage);
					}
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in CheckAutoLoginAsync(): {ex}");
			}
		}
	}
}