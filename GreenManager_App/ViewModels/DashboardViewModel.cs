using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using GreenManager_App.Views.Customers;
using GreenManager_App.Views.Projects;
using GreenManager_App.Views.Materials;
using GreenManager_App.Views.Employees;

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

				var customersPage = _serviceProvider.GetRequiredService<CustomersPage>();
				await window.Page.Navigation.PushAsync(customersPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToCustomersAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToProjectsAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var projectsPage = _serviceProvider.GetRequiredService<ProjectsPage>();
				await window.Page.Navigation.PushAsync(projectsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToProjectsAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToMaterialsAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var materialsPage = _serviceProvider.GetRequiredService<MaterialsPage>();
				await window.Page.Navigation.PushAsync(materialsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToMaterialsAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToEmployeesAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var materialsPage = _serviceProvider.GetRequiredService<EmployeesPage>();
				await window.Page.Navigation.PushAsync(materialsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToMaterialsAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToSettingsAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var settingsPage = _serviceProvider.GetRequiredService<SettingsPage>();
				await window.Page.Navigation.PushAsync(settingsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToSettingsAsync(): {ex}");
			}
		}
	}
}