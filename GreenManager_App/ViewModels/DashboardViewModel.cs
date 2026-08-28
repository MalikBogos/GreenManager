using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using GreenManager_App.Views.Customers;
using GreenManager_App.Views.Projects;
using GreenManager_App.Views.Materials;
using GreenManager_App.Views.Employees;

namespace GreenManager_App.ViewModels
{
	/// <summary>
	/// ViewModel voor de DashboardPage (MVVM). Beheert de navigatielogica van het hoofdscherm (Dashboard) naar de verschillende overzichtspagina's van de applicatie (Klanten, Projecten, Materiaal, Werknemers en Instellingen).
	/// </summary>
	public partial class DashboardViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		public DashboardViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Navigeert van het Dashboard naar de CustomersPage.
		/// </summary>
		[RelayCommand]
		public Task NavigateToCustomersAsync() => NavigateToAsync<CustomersPage>();

		/// <summary>
		/// Navigeert van het Dashboard naar de ProjectsPage.
		/// </summary>
		[RelayCommand]
		public Task NavigateToProjectsAsync() => NavigateToAsync<ProjectsPage>();

		/// <summary>
		/// Navigeert van het Dashboard naar de MaterialsPage.
		/// </summary>
		[RelayCommand]
		public Task NavigateToMaterialsAsync() => NavigateToAsync<MaterialsPage>();

		/// <summary>
		/// Navigeert van het Dashboard naar de EmployeesPage.
		/// </summary>
		[RelayCommand]
		public Task NavigateToEmployeesAsync() => NavigateToAsync<EmployeesPage>();

		/// <summary>
		/// Navigeert van het Dashboard naar de SettingsPage.
		/// </summary>
		[RelayCommand]
		public Task NavigateToSettingsAsync() => NavigateToAsync<SettingsPage>();

		/// <summary>
		/// Generieke helper voor pagina-navigatie om herhaling te vermijden.
		/// </summary>
		/// <typeparam name="TPage">De pagina waarnaar genavigeerd moet worden.</typeparam>
		private async Task NavigateToAsync<TPage>() where TPage : Page
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var page = _serviceProvider.GetRequiredService<TPage>();
				await window.Page.Navigation.PushAsync(page);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToAsync<{typeof(TPage).Name}>(): {ex}");
			}
		}
	}
}