using GreenManager_App.ViewModels;
using GreenManager_App.Services;
using GreenManager_App.Views;
using GreenManager_App.Views.Customers;
using GreenManager_App.Views.Materials;
using Microsoft.Extensions.Logging;
using GreenManager_App.Views.Projects;
using GreenManager_App.Views.Employees;

namespace GreenManager_App
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			//  Service & ViewModels
			builder.Services.AddSingleton<ApiService>();
			builder.Services.AddSingleton<CustomersViewModel>();
			builder.Services.AddSingleton<ProjectsViewModel>();
			builder.Services.AddSingleton<MaterialsViewModel>();
			builder.Services.AddSingleton<EmployeesViewModel>();

			// ViewModels & Views
			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<LoginPage>();

			builder.Services.AddTransient<DashboardViewModel>();
			builder.Services.AddTransient<DashboardPage>();

			builder.Services.AddTransient<SettingsViewModel>();
			builder.Services.AddTransient<SettingsPage>();
			// Klanten
			builder.Services.AddTransient<CustomersPage>();
			builder.Services.AddTransient<AddCustomerPage>();
			builder.Services.AddTransient<CustomerDetailsPage>();
			builder.Services.AddTransient<EditCustomerPage>();

			// Projecten
			builder.Services.AddTransient<ProjectsPage>();
			builder.Services.AddTransient<AddProjectPage>();
			builder.Services.AddTransient<ProjectDetailsPage>();
			builder.Services.AddTransient<EditProjectPage>();


			// Materiaal
			builder.Services.AddTransient<MaterialsPage>();
			builder.Services.AddTransient<AddMaterialPage>();
			builder.Services.AddTransient<MaterialDetailsPage>();
			builder.Services.AddTransient<EditMaterialPage>();

			// Werknemers
			builder.Services.AddTransient<EmployeesPage>();
			builder.Services.AddTransient<AddEmployeePage>();
			builder.Services.AddTransient<EmployeeDetailsPage>();
			builder.Services.AddTransient<EditEmployeePage>();



			return builder.Build();
		}
	}
}
