using Models.Entities;
using Models.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using System.Collections.ObjectModel;

namespace GreenManager_App.ViewModels
{
	public partial class ProjectsViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		public ObservableCollection<ProjectDto> Projects { get; set; } = new ObservableCollection<ProjectDto>();
		public ObservableCollection<Customer> CustomersForPicker { get; set; } = new ObservableCollection<Customer>();

		[ObservableProperty] public partial bool IsBusy { get; set; }
		[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

		[ObservableProperty] public partial string NewName { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewDescription { get; set; } = string.Empty;
		[ObservableProperty] public partial DateTime NewStartDate { get; set; } = DateTime.Today;
		[ObservableProperty] public partial DateTime? NewEndDate { get; set; }
		[ObservableProperty] public partial string NewProjectAddress { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewBudget { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewNotes { get; set; } = string.Empty;

		[ObservableProperty] public partial Customer? SelectedCustomerForNewProject { get; set; }
		// Het geselecteerde project voor de Details en Edit pagina
		[ObservableProperty] public partial ProjectDto? SelectedProject { get; set; }

		[ObservableProperty] public partial Customer? SelectedCustomerForEdit { get; set; }

		public ProjectsViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		[RelayCommand]
		public async Task LoadProjectsAsync()
		{
			if (IsBusy) return;
			try
			{
				IsBusy = true;
				var data = await _apiService.GetProjectsAsync();
				Projects.Clear();
				if (data != null)
				{
					foreach (var project in data) Projects.Add(project);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in LoadProjectsAsync(): {ex}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		public async Task NavigateToAddProjectAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var customers = await _apiService.GetCustomersAsync();
				CustomersForPicker.Clear();
				if (customers != null)
				{
					foreach (var c in customers) CustomersForPicker.Add(c);
				}

				// Maak velden schoon
				NewName = string.Empty; NewDescription = string.Empty;
				NewStartDate = DateTime.Today; NewEndDate = null;
				NewBudget = string.Empty; NewNotes = string.Empty;
				SelectedCustomerForNewProject = null; ErrorMessage = string.Empty;

				var addPage = _serviceProvider.GetRequiredService<Views.Projects.AddProjectPage>();
				addPage.BindingContext = this;

				await window.Page.Navigation.PushAsync(addPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToAddProjectAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task SaveProjectAsync()
		{
			try
			{
				ErrorMessage = string.Empty;
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				if (string.IsNullOrWhiteSpace(NewName) || SelectedCustomerForNewProject == null)
				{
					ErrorMessage = "Projectnaam en een gekozen Klant zijn verplicht.";
					return;
				}

				decimal.TryParse(NewBudget, out decimal budgetAmount);

				var requestDto = new ProjectRequestDto
				{
					Name = NewName,
					Description = NewDescription,
					StartDate = NewStartDate,
					EndDate = NewEndDate,
					Budget = budgetAmount,
					Notes = NewNotes,
					CustomerId = SelectedCustomerForNewProject.Id,
					Status = "Quotation", // Tekst in plaats van Enum
					ProjectAddress = NewProjectAddress
				};

				bool success = await _apiService.CreateProjectAsync(requestDto);

				if (success)
				{
					await window.Page.Navigation.PopAsync();
					await LoadProjectsAsync();
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Opslaan mislukt. Controleer de gegevens of de serververbinding.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in SaveProjectAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToDetailsAsync(ProjectDto selected)
		{
			try
			{
				if (selected == null) return;
				SelectedProject = selected;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var detailsPage = _serviceProvider.GetRequiredService<Views.Projects.ProjectDetailsPage>();
				detailsPage.BindingContext = this;

				await window.Page.Navigation.PushAsync(detailsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToDetailsAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task DeleteProjectAsync()
		{
			try
			{
				if (SelectedProject == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				bool isConfirmed = await window.Page.DisplayAlertAsync("Verwijderen", $"Weet je zeker dat je {SelectedProject.Name} wilt verwijderen?", "Ja", "Nee");
				if (!isConfirmed) return;

				bool isSuccess = await _apiService.DeleteProjectAsync(SelectedProject.Id);

				if (isSuccess)
				{
					await window.Page.Navigation.PopAsync();
					SelectedProject = null;
					await LoadProjectsAsync();
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Project kon niet worden verwijderd.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in DeleteProjectAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToEditProjectAsync()
		{
			try
			{
				if (SelectedProject == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				// Laad de klanten voor de picker en selecteer de huidige klant
				var customers = await _apiService.GetCustomersAsync();
				CustomersForPicker.Clear();
				if (customers != null)
				{
					foreach (var c in customers) CustomersForPicker.Add(c);
				}

				SelectedCustomerForEdit = CustomersForPicker.FirstOrDefault(c => c.Id == SelectedProject.CustomerId);

				var editPage = _serviceProvider.GetRequiredService<Views.Projects.EditProjectPage>();
				editPage.BindingContext = this;

				await window.Page.Navigation.PushAsync(editPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToEditProjectAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task UpdateProjectAsync()
		{
			try
			{
				ErrorMessage = string.Empty;
				if (SelectedProject == null || SelectedCustomerForEdit == null)
				{
					ErrorMessage = "Selecteer een geldige klant en vul de projectnaam in.";
					return;
				}

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				if (string.IsNullOrWhiteSpace(SelectedProject.Name))
				{
					ErrorMessage = "Projectnaam is verplicht.";
					return;
				}

				// Gebruik de DTO voor de update request
				var requestDto = new ProjectRequestDto
				{
					Name = SelectedProject.Name,
					Description = SelectedProject.Description,
					StartDate = SelectedProject.StartDate,
					EndDate = SelectedProject.EndDate,
					Budget = SelectedProject.Budget,
					Notes = SelectedProject.Notes,
					CustomerId = SelectedCustomerForEdit.Id,
					Status = SelectedProject.Status,
					ProjectAddress = SelectedProject.ProjectAddress
				};

				bool isSuccess = await _apiService.UpdateProjectAsync(SelectedProject.Id, requestDto);

				if (isSuccess)
				{
					await window.Page.Navigation.PopAsync();
					int currentId = SelectedProject.Id;
					await LoadProjectsAsync();
					SelectedProject = Projects.FirstOrDefault(p => p.Id == currentId);
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Project kon niet worden aangepast.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in UpdateProjectAsync(): {ex}");
			}
		}
	}
}