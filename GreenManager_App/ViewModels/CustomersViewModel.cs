using Models.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using System.Collections.ObjectModel;
using GreenManager_App.Views.Customers;

namespace GreenManager_App.ViewModels
{
	/// <summary>
	/// ViewModel voor de CustomersPage (MVVM). Beheert de CRUD-operations en filteren van klanten via de ApiService. Beheert ook de navigatie tussen de DashboardPage, het aanmaak-formulier (AddCustomerPage), de detailpagina (CustomerDetailsPage) en het bewerk-formulier (EditCustomerPage). Wordt gedeeld tussen meerdere pagina's via dependency injection.
	/// </summary>
	public partial class CustomersViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Gefilterde lijst van klanten gevuld met ApplyFilter. Gebonden aan UI via bindings.
		/// </summary>
		public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

		[ObservableProperty]
		public partial bool IsBusy { get; set; }
		[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

		/// <summary>
		/// De klant die momenteel geselecteerd is voor de CRUD op een klant. Wordt gedeeld tussen de verschillende pagina's.
		/// </summary>
		[ObservableProperty] public partial Customer? SelectedCustomer { get; set; }

		// Nieuwe klant eigenschappen. Dit zijn velden die via de Bindings op AddCustomerPage worden gebruikt om een nieuwe klant aan te maken.
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

		// Filter
		/// <summary>
		/// De volledige lijst van klanten zoals laatst opgehaald via de API. Dient als bron voor ApplyFilter.
		/// </summary>
		private List<Customer> _allCustomers = new List<Customer>();
		/// <summary>
		/// De mogelijke filterwaarden die in de UI getoond worden.
		/// </summary>
		public List<string> FilterOptions { get; } = new List<string> { "Alle Klanten", "Bedrijven", "Particulieren" };

		/// <summary>
		/// De momenteel geselecteerde filter. Bij wijziging wordt automatisch ApplyFilter uitgevoerd.
		/// </summary>
		[ObservableProperty]
		public partial string SelectedFilter { get; set; }

		public CustomersViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
			SelectedFilter = "Alle Klanten"; // Standaard filter instellen
		}

		/// <summary>
		/// Wordt automatisch aangeroepen door de CommunityToolkit.Mvvm source generator wanneer SelectedFilter wijzigt en past het filter toe op de weergegeven lijst.
		/// </summary>
		partial void OnSelectedFilterChanged(string value)
		{
			ApplyFilter();
		}

		/// <summary>
		/// Haalt alle klanten op via de API en slaat ze op in _allCustomers en past vervolgens de huidige filter toe om Customers te vullen. Doet niets als IsBusy == true.
		/// </summary>
		[RelayCommand]
		public async Task LoadCustomersAsync()
		{
			if (IsBusy) return;
			try
			{
				IsBusy = true;
				var data = await _apiService.GetCustomersAsync();

				if (data != null)
				{
					// Sla alle data op in de backup-lijst
					_allCustomers = data.ToList();
				}

				// Pas direct het filter toe om de UI-lijst te vullen
				ApplyFilter();
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

		/// <summary>
		/// Filtert _allCustomers op basis van SelectedFilter ("Alle Klanten", "Bedrijven" of "Particulieren", waarbij het onderscheid wordt gemaakt op basis van of CompanyName ingevuld is) en herbouwt Customers.
		/// </summary>
		private void ApplyFilter()
		{
			Customers.Clear();
			var filtered = _allCustomers.AsEnumerable();

			if (SelectedFilter == "Bedrijven")
			{
				filtered = filtered.Where(c => !string.IsNullOrWhiteSpace(c.CompanyName));
			}
			else if (SelectedFilter == "Particulieren")
			{
				filtered = filtered.Where(c => string.IsNullOrWhiteSpace(c.CompanyName));
			}

			foreach (var customer in filtered)
			{
				Customers.Add(customer);
			}
		}
		
		/// <summary>
		/// Navigeert van het klantenoverzicht (CustomersPage) naar de AddCustomerPage.
		/// </summary>
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

		/// <summary>
		/// Valideert de ingevulde "Nieuwe klant"-velden en maakt bij succes een nieuwe klant aan via de API. Wist bij succes de invoervelden en navigeert terug naar het overzicht (CustomersPage).
		/// </summary>
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

		/// <summary>
		/// Slaat de geselecteerde klant op in SelectedCustomer en navigeert naar de CustomerDetailsPage, die deze klant via de gedeelde ViewModel kan lezen.
		/// </summary>
		/// <param name="selected">De klant waarvan de details getoond moeten worden.</param>
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

		/// <summary>
		/// Navigeert naar de EditCustomerPage voor de klant die al in SelectedCustomer staat. Doet niets als er geen klant geselecteerd is.
		/// </summary>
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
				Console.WriteLine($"Fout in NavigateToEditCustomerAsync(): {ex}");
			}
		}

		/// <summary>
		/// Valideert en werkt de gegevens van SelectedCustomer bij via de API. Navigeert bij succes terug, herlaadt de klantenlijst en stelt SelectedCustomer opnieuw in op de bijgewerkte versie van diezelfde klant.
		/// </summary>
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
				Console.WriteLine($"Fout in UpdateCustomerAsync(): {ex}");
			}
		}

		/// <summary>
		/// Vraagt bevestiging aan de gebruiker en verwijdert (soft-delete via de API) de klant in SelectedCustomer. Navigeert bij succes terug naar de CustomersPage en herlaadt de klantenlijst.
		/// </summary>
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