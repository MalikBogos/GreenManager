using Models.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using System.Collections.ObjectModel;
using GreenManager_App.Views.Customers;

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
		[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

		[ObservableProperty] public partial Customer? SelectedCustomer { get; set; }

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

		public CustomersViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		[RelayCommand]
		public async Task LoadCustomersAsync()
		{
			if (IsBusy) return;
			try
			{
				IsBusy = true;
				var data = await _apiService.GetCustomersAsync();
				Customers.Clear();
				if (data != null)
				{
					foreach (var customer in data) Customers.Add(customer);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in LoadCustomersAsync(): {ex}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		// Opent AddCustomerPage

		[RelayCommand]
		public async Task NavigateToAddCustomerAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var addPage = _serviceProvider.GetRequiredService<AddCustomerPage>();
				await window.Page.Navigation.PushAsync(addPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToAddCustomerAsync(): {ex}");
			}
		}

		[RelayCommand]
        public async Task SaveCustomerAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                if (string.IsNullOrWhiteSpace(NewFirstName) || string.IsNullOrWhiteSpace(NewLastName) || 
                    string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(NewPhoneNumber))
                {
                    ErrorMessage = "Voornaam, Achternaam, E-mail en Telefoonnummer zijn verplicht.";
                    return;
                }

                var newCustomer = new Customer
                {
                    FirstName = NewFirstName, LastName = NewLastName, CompanyName = NewCompanyName,
                    VATNumber = NewVATNumber, Email = NewEmail, PhoneNumber = NewPhoneNumber,
                    Street = NewStreet, PostalCode = NewPostalCode, City = NewCity, Notes = NewNotes
                };

                bool success = await _apiService.CreateCustomerAsync(newCustomer);

                if (success)
                {
                    // Wis velden
                    NewFirstName = string.Empty; NewLastName = string.Empty; NewCompanyName = string.Empty;
                    NewVATNumber = string.Empty; NewEmail = string.Empty; NewPhoneNumber = string.Empty;
                    NewStreet = string.Empty; NewPostalCode = string.Empty; NewCity = string.Empty; NewNotes = string.Empty;

                    var window = Application.Current?.Windows.FirstOrDefault();
					if (window?.Page != null) { await window.Page.Navigation.PopAsync(); }
				}
                else
                {
                    ErrorMessage = "Opslaan mislukt.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout in SaveCustomerAsync(): {ex}");
            }
        }

		[RelayCommand]
		public async Task NavigateToDetailsAsync(Customer selected)
		{
			try
			{
				if (selected == null) return;

				SelectedCustomer = selected; // Sla op zodat de detailpagina dit kan gebruiken

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var detailsPage = _serviceProvider.GetRequiredService<CustomerDetailsPage>();
				await window.Page.Navigation.PushAsync(detailsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToDetailsAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToEditCustomerAsync()
		{
			try
			{
				if (SelectedCustomer == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var editPage = _serviceProvider.GetRequiredService<EditCustomerPage>();
				await window.Page.Navigation.PushAsync(editPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToEditCustomerAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task UpdateCustomerAsync()
		{
			try
			{
				ErrorMessage = string.Empty;
				if (SelectedCustomer == null) return;

				// Controleer of de vereiste velden niet leeg zijn gemaakt door de gebruiker
				if (string.IsNullOrWhiteSpace(SelectedCustomer.FirstName) ||
					string.IsNullOrWhiteSpace(SelectedCustomer.LastName) ||
					string.IsNullOrWhiteSpace(SelectedCustomer.Email) ||
					string.IsNullOrWhiteSpace(SelectedCustomer.PhoneNumber))
				{
					ErrorMessage = "Voornaam, Achternaam, E-mail en Telefoonnummer zijn verplicht.";
					return;
				}

				bool isSuccess = await _apiService.UpdateCustomerAsync(SelectedCustomer.Id, SelectedCustomer);

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				if (isSuccess)
				{
					await window.Page.Navigation.PopAsync();

					int currentCustomerId = SelectedCustomer.Id;

					await LoadCustomersAsync();

					SelectedCustomer = Customers.FirstOrDefault(c => c.Id == currentCustomerId);
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "De klant kon niet worden aangepast.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in UpdateCustomerAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task DeleteCustomerAsync()
		{
			try
			{
				if (SelectedCustomer == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				bool confirm = await window.Page.DisplayAlertAsync("Verwijderen", $"Weet je zeker dat je {SelectedCustomer.FirstName} wilt verwijderen?", "Ja", "Nee");
				if (!confirm) return;

				bool success = await _apiService.DeleteCustomerAsync(SelectedCustomer.Id);

				if (success)
				{
					// Ga veilig terug naar het klantenoverzicht
					await window.Page.Navigation.PopAsync();

					SelectedCustomer = null;
					await LoadCustomersAsync();
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Klant kon niet worden verwijderd.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in DeleteCustomerAsync(): {ex}");
			}
		}
	}
}