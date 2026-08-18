using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Models.Data;
using Models.Entities;
using GreenManager_WPF.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using GreenManager_WPF.Views;

namespace GreenManager_WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IHost AppHost { get; private set; }

		public App()
		{
			AppHost = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
			{
				string defaultConnection = "Server=(localdb)\\MSSQLLocalDB;Database=GreenManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true";

				services.AddDbContext<GreenManagerDbContext>(options => options.UseSqlServer(defaultConnection), contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton);

				services.AddDbContextFactory<GreenManagerDbContext>(options => options.UseSqlServer(defaultConnection));

				services.AddIdentityCore<ApplicationUser>(options =>
				{
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequiredLength = 3;
				}).AddRoles<IdentityRole>().AddEntityFrameworkStores<GreenManagerDbContext>();

				services.AddTransient<MainViewModel>();
				services.AddTransient<LoginViewModel>();
				services.AddTransient<DashboardViewModel>();
				services.AddTransient<CustomerViewModel>();
				services.AddTransient<EmployeeViewModel>();
				services.AddTransient<MaterialViewModel>();
				services.AddTransient<ProjectViewModel>();
				services.AddTransient<RegisterViewModel>();

				services.AddTransient<LoginWindow>();
				services.AddTransient<MainWindow>();
				services.AddTransient<RegisterWindow>();
				services.AddTransient<CustomerView>();
				services.AddTransient<DashboardView>();
				services.AddTransient<EmployeeView>();
				services.AddTransient<MaterialView>();
				services.AddTransient<ProjectView>();

			}).Build();
			
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			try
			{
				await AppHost.StartAsync();

				var loginWindow = AppHost.Services.GetRequiredService<LoginWindow>();
				loginWindow.Show();
			} catch (Exception ex)
			{
				MessageBox.Show($"De fout '{ex.Message}' is opgetreden.", "Foutmelding", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			var culture = new CultureInfo("nl-BE");

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			FrameworkElement.LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
		}

		protected override async void OnExit (ExitEventArgs e)
		{
			await AppHost.StopAsync();
			AppHost.Dispose();
			base.OnExit(e);
		}
	}

}
