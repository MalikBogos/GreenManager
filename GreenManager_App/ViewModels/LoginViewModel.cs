using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;

namespace GreenManager_App.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly IServiceProvider _serviceProvider;

		[ObservableProperty]
		public partial string Email { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string Password { get; set; } = string.Empty;

		[ObservableProperty]
		public partial string ErrorMessage { get; set; } = string.Empty;

		[ObservableProperty]
		public partial bool HasError { get; set; }

        public LoginViewModel(ApiService apiService, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _serviceProvider = serviceProvider;
        }

		[RelayCommand]
		public async Task LoginAsync()
		{
			HasError = false;
			ErrorMessage = string.Empty;

			if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
			{
				ErrorMessage = "Vul e-mail en wachtwoord in.";
				HasError = true;
				return;
			}

			// Probeer in te loggen via de API (gebeurt op de achtergrond)
			bool isSuccess = await _apiService.LoginAsync(Email, Password);

			try
			{
				if (isSuccess)
				{
					// Roep de Main Thread aan om het scherm veilig te wisselen
					MainThread.BeginInvokeOnMainThread(() =>
					{
						Page rootPage;
						var customersPage = _serviceProvider.GetRequiredService<Views.CustomersPage>();

						rootPage = new NavigationPage(customersPage);
					});
				}
				else
				{
					ErrorMessage = "Inloggen mislukt. Controleer je gegevens of accountstatus.";
					HasError = true;
				}
			} catch (Exception ex)
			{
				Console.WriteLine($"De fout {ex} is opgetreden");
			}
			
		}
	}
}