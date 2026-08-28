using Models.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using System.Collections.ObjectModel;
using GreenManager_App.Views.Materials;

namespace GreenManager_App.ViewModels
{
	/// <summary>
	/// ViewModel voor de MaterialsPage (MVVM). Beheert de CRUD-operations op materiaal via de ApiService. Beheert ook de navigatie tussen het materiaaloverzicht (MaterialsPage), het aanmaak-formulier (AddMaterialPage), de detailpagina (MaterialDetailsPage) en het bewerk-formulier (EditMaterialPage). Wordt gedeeld tussen meerdere pagina's via dependency injection.
	/// </summary>
	public partial class MaterialsViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Lijst van materiaal, gebonden aan de UI via bindings.
		/// </summary>
		public ObservableCollection<Material> Materials { get; set; } = new ObservableCollection<Material>();

		[ObservableProperty] public partial bool IsBusy { get; set; }
		[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

		/// <summary>
		/// Het materiaal dat momenteel geselecteerd is voor de CRUD op een materiaal. Wordt gedeeld tussen de verschillende pagina's.
		/// </summary>
		[ObservableProperty] public partial Material? SelectedMaterial { get; set; }

		// Velden voor nieuw materiaal. Dit zijn velden die via de Bindings op AddMaterialPage worden gebruikt om een nieuw materiaal aan te maken.
		[ObservableProperty] public partial string NewName { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewDescription { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewUnit { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewPurchasePrice { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewStockQuantity { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewNotes { get; set; } = string.Empty;

		public MaterialsViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Haalt al het materiaal op via de API en slaat ze op in Materials. Doet niets als IsBusy == true.
		/// </summary>
		[RelayCommand]
		public async Task LoadMaterialsAsync()
		{
			if (IsBusy) return;
			try
			{
				IsBusy = true;
				var data = await _apiService.GetMaterialsAsync();
				Materials.Clear();
				if (data != null)
				{
					foreach (var material in data) Materials.Add(material);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in LoadMaterialsAsync(): {ex}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		/// <summary>
		/// Navigeert van het materiaaloverzicht (MaterialsPage) naar de AddMaterialPage.
		/// </summary>
		[RelayCommand]
		public async Task NavigateToAddMaterialAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var addPage = _serviceProvider.GetRequiredService<AddMaterialPage>();
				addPage.BindingContext = this;
				await window.Page.Navigation.PushAsync(addPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToAddMaterialAsync(): {ex}");
			}
		}

		/// <summary>
		/// Valideert de ingevulde "Nieuw materiaal"-velden en maakt bij succes een nieuw materiaal aan via de API. Wist bij succes de invoervelden en navigeert terug naar het overzicht (MaterialsPage).
		/// </summary>
		[RelayCommand]
		public async Task SaveMaterialAsync()
		{
			try
			{
				ErrorMessage = string.Empty;
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				if (string.IsNullOrWhiteSpace(NewName))
				{
					ErrorMessage = "Naam is verplicht.";
					return;
				}

				// Veilige conversie van tekst naar decimal
				decimal.TryParse(NewPurchasePrice, out decimal price);
				decimal.TryParse(NewStockQuantity, out decimal quantity);

				var newMaterial = new Material
				{
					Name = NewName,
					Description = NewDescription,
					Unit = NewUnit,
					PurchasePrice = price,
					StockQuantity = quantity,
					Notes = NewNotes
				};

				bool success = await _apiService.CreateMaterialAsync(newMaterial);

				if (success)
				{
					NewName = string.Empty; NewDescription = string.Empty; NewUnit = string.Empty;
					NewPurchasePrice = string.Empty; NewStockQuantity = string.Empty; NewNotes = string.Empty;

					await window.Page.Navigation.PopAsync();
					await LoadMaterialsAsync();
				}
				else
				{
					ErrorMessage = "Opslaan mislukt.";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in SaveMaterialAsync(): {ex}");
			}
		}

		/// <summary>
		/// Slaat het geselecteerde materiaal op in SelectedCustomer en navigeert naar de MaterialDetailsPage, die dit materiaal via de gedeelde ViewModel kan lezen.
		/// </summary>
		/// <param name="selected">Het materiaal waarvan de details getoond moeten worden.</param>
		[RelayCommand]
		public async Task NavigateToDetailsAsync(Material selected)
		{
			try
			{
				if (selected == null) return;
				SelectedMaterial = selected;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var detailsPage = _serviceProvider.GetRequiredService<MaterialDetailsPage>();
				detailsPage.BindingContext = this;
				await window.Page.Navigation.PushAsync(detailsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToDetailsAsync(): {ex}");
			}
		}

		/// <summary>
		/// Navigeert naar de EditMaterialPage voor het materiaal die al in SelectedMaterial staat. Doet niets als er geen materiaal geselecteerd is.
		/// </summary>
		[RelayCommand]
		public async Task NavigateToEditMaterialAsync()
		{
			try
			{
				if (SelectedMaterial == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var editPage = _serviceProvider.GetRequiredService<EditMaterialPage>();
				editPage.BindingContext = this;
				await window.Page.Navigation.PushAsync(editPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in NavigateToEditMaterialAsync(): {ex}");
			}
		}

		/// <summary>
		/// Valideert en werkt de gegevens van SelectedMaterial bij via de API. Navigeert bij succes terug, herlaadt de materiaallijst en stelt SelectedMaterial opnieuw in op de bijgewerkte versie van datzelfde materiaal.
		/// </summary>
		[RelayCommand]
		public async Task UpdateMaterialAsync()
		{
			try
			{
				ErrorMessage = string.Empty;
				if (SelectedMaterial == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				if (string.IsNullOrWhiteSpace(SelectedMaterial.Name))
				{
					ErrorMessage = "Naam is verplicht.";
					return;
				}

				bool isSuccess = await _apiService.UpdateMaterialAsync(SelectedMaterial.Id, SelectedMaterial);

				if (isSuccess)
				{
					await window.Page.Navigation.PopAsync();
					int currentId = SelectedMaterial.Id;
					await LoadMaterialsAsync();
					SelectedMaterial = Materials.FirstOrDefault(m => m.Id == currentId);
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Materiaal kon niet worden aangepast.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in UpdateMaterialAsync(): {ex}");
			}
		}

		/// <summary>
		/// Vraagt bevestiging aan de gebruiker en verwijdert (soft-delete via de API) het materiaal in SelectedMaterial. Navigeert bij succes terug naar de MaterialsPage en herlaadt de materiaallijst.
		/// </summary>
		[RelayCommand]
		public async Task DeleteMaterialAsync()
		{
			try
			{
				if (SelectedMaterial == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				bool isConfirmed = await window.Page.DisplayAlertAsync("Verwijderen", $"Weet je zeker dat je {SelectedMaterial.Name} wilt verwijderen?", "Ja", "Nee");
				if (!isConfirmed) return;

				bool isSuccess = await _apiService.DeleteMaterialAsync(SelectedMaterial.Id);

				if (isSuccess)
				{
					await window.Page.Navigation.PopAsync();
					SelectedMaterial = null;
					await LoadMaterialsAsync();
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Materiaal kon niet worden verwijderd.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout in DeleteMaterialAsync(): {ex}");
			}
		}
	}
}