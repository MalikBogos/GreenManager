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

		public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

		// Deze variabele gebruiken we om een laad-icoon te tonen
		[ObservableProperty]
		public partial bool IsBusy { get; set; }

		public CustomersViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		// Dit wordt aangeroepen zodra het scherm opent
		[RelayCommand]
		public async Task LoadCustomersAsync()
		{
			// Voorkom dat we 2 keer tegelijk laden (bv. door een dubbele 'klik') 
			if (IsBusy) return;

			try
			{
				IsBusy = true;

				// Haal de data online op via de ApiService
				var data = await _apiService.GetCustomersAsync();

				// Maak de lijst op het scherm leeg en vul hem met de nieuwe klanten
				Customers.Clear();
				if (data != null)
				{
					foreach (var customer in data)
					{
						Customers.Add(customer);
					}
				}
			}
			finally
			{
				IsBusy = false; // Verberg het laad-icoon weer
			}
		}

		[RelayCommand]
		public void Logout()
		{
			// Wis de gegevens in de service
			_apiService.Logout();
			Page rootPage;

			// Open opnieuw de inlogpagina
			MainThread.BeginInvokeOnMainThread(() =>
			{
				var loginPage = _serviceProvider.GetRequiredService<Views.LoginPage>();
				rootPage = new NavigationPage(loginPage);
			});
		}
	}
}