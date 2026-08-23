using Models.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using System.Collections.ObjectModel;
using GreenManager_App.Views.Materials;

namespace GreenManager_App.ViewModels
{
	public partial class MaterialsViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		public ObservableCollection<Material> Materials { get; set; } = new ObservableCollection<Material>();

		[ObservableProperty] public partial bool IsBusy { get; set; }
		[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

		[ObservableProperty] public partial Material? SelectedMaterial { get; set; }

		// Velden voor nieuw materiaal (strings voor veilige TryParse)
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
				Console.WriteLine($"Error in NavigateToEditMaterialAsync(): {ex}");
			}
		}

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
				Console.WriteLine($"Error in UpdateMaterialAsync(): {ex}");
			}
		}

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
				Console.WriteLine($"Error in DeleteMaterialAsync(): {ex}");
			}
		}
	}
}