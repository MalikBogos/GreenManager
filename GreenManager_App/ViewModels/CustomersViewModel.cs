using Models.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using System.Collections.ObjectModel;

namespace GreenManager_App.ViewModels
{
	public partial class CustomersViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		// --- OVERZICHT EIGENSCHAPPEN ---
		public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

		[ObservableProperty]
		public partial bool IsBusy { get; set; }

		// --- NIEUWE KLANT EIGENSCHAPPEN ---
		[ObservableProperty] public partial string NewFirstName { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewLastName { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewCompanyName { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewVATNumber { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewEmail { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewPhoneNumber { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewStreet { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewPostalCode { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewCity { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewNotes { get; set; } = string.Empty;

		[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

		public CustomersViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		[RelayCommand]
		public async Task LoadCustomersAsync()
		{
			if (IsBusy) return;
			IsBusy = true;

			var data = await _apiService.GetCustomersAsync();
			Customers.Clear();
			if (data != null)
			{
				foreach (var customer in data)
				{
					Customers.Add(customer);
				}
			}
			IsBusy = false;
		}

		// Opent AddCustomerPage

		[RelayCommand]
		public void NavigateToAddCustomer()
		{
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page != null)
				{
					// Haal de nieuwe pagina op en navigeer ernaartoe
					var addPage = _serviceProvider.GetRequiredService<Views.AddCustomerPage>();
					await window.Page.Navigation.PushAsync(addPage);
				}
			});
		}

		[RelayCommand]
		public async Task SaveCustomerAsync()
		{
			ErrorMessage = string.Empty;

			// Vereiste invulvelden
			if (string.IsNullOrWhiteSpace(NewFirstName) ||
				string.IsNullOrWhiteSpace(NewLastName) ||
				string.IsNullOrWhiteSpace(NewEmail) ||
				string.IsNullOrWhiteSpace(NewPhoneNumber))
			{
				ErrorMessage = "Voornaam, Achternaam, E-mail en Telefoonnummer zijn verplicht.";
				return;
			}

			// Koppel alle MAUI velden aan het originele Customer model
			var newCustomer = new Customer
			{
				FirstName = NewFirstName,
				LastName = NewLastName,
				CompanyName = NewCompanyName,
				VATNumber = NewVATNumber,
				Email = NewEmail,
				PhoneNumber = NewPhoneNumber,
				Street = NewStreet,
				PostalCode = NewPostalCode,
				City = NewCity,
				Notes = NewNotes
			};

			// Stuur het naar de server
			bool success = await _apiService.CreateCustomerAsync(newCustomer);

			if (success)
			{
				// Wis alle velden voor de volgende keer
				NewFirstName = string.Empty;
				NewLastName = string.Empty;
				NewCompanyName = string.Empty;
				NewVATNumber = string.Empty;
				NewEmail = string.Empty;
				NewPhoneNumber = string.Empty;
				NewStreet = string.Empty;
				NewPostalCode = string.Empty;
				NewCity = string.Empty;
				NewNotes = string.Empty;

				// Ga veilig terug naar het overzicht
				MainThread.BeginInvokeOnMainThread(async () =>
				{
					var window = Application.Current?.Windows.FirstOrDefault();
					if (window?.Page != null)
					{
						await window.Page.Navigation.PopAsync();
					}
				});
			}
			else
			{
				ErrorMessage = "Opslaan mislukt. Controleer je gegevens of de serververbinding.";
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