using GreenManager_App.ViewModels;
using GreenManager_App.Views;
using Microsoft.Extensions.Logging;

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

			// Registreer de service
			builder.Services.AddSingleton<Services.ApiService>();

			// Registreer de ViewModels & Views
			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<LoginPage>();

			builder.Services.AddTransient<CustomersViewModel>();
			builder.Services.AddTransient<CustomersPage>();

			return builder.Build();
		}
	}
}
