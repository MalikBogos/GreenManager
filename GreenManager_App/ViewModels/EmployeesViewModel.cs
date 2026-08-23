using Models.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_App.Services;
using System.Collections.ObjectModel;

namespace GreenManager_App.ViewModels
{
	public partial class EmployeesViewModel : ObservableObject
	{
		private readonly ApiService _apiService;
		private readonly IServiceProvider _serviceProvider;

		public ObservableCollection<EmployeeDto> Employees { get; set; } = new ObservableCollection<EmployeeDto>();

		[ObservableProperty] public partial bool IsBusy { get; set; }
		[ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

		[ObservableProperty] public partial EmployeeDto? SelectedEmployee { get; set; }

		// Properties voor het toevoegen van een nieuwe werknemer
		[ObservableProperty] public partial string NewFirstName { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewLastName { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewEmail { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewEmployeeNumber { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewJobTitle { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewHourlyWage { get; set; } = string.Empty;
		[ObservableProperty] public partial DateTime NewHireDate { get; set; } = DateTime.Today;
		[ObservableProperty] public partial DateTime? NewDateOfBirth { get; set; }
		[ObservableProperty] public partial string NewStreet { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewPostalCode { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewCity { get; set; } = string.Empty;
		[ObservableProperty] public partial string NewNotes { get; set; } = string.Empty;

		public EmployeesViewModel(ApiService apiService, IServiceProvider serviceProvider)
		{
			_apiService = apiService;
			_serviceProvider = serviceProvider;
		}

		[RelayCommand]
		public async Task LoadEmployeesAsync()
		{
			if (IsBusy) return;
			try
			{
				IsBusy = true;
				var data = await _apiService.GetEmployeesAsync();
				Employees.Clear();
				if (data != null)
				{
					foreach (var emp in data) Employees.Add(emp);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in LoadEmployeesAsync(): {ex}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		[RelayCommand]
		public async Task NavigateToAddEmployeeAsync()
		{
			try
			{
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				// Reset velden
				NewFirstName = string.Empty; NewLastName = string.Empty; NewEmail = string.Empty;
				NewEmployeeNumber = $"EMP{new Random().Next(100, 999)}"; // Simpele nummer-generator
				NewJobTitle = string.Empty; NewHourlyWage = string.Empty;
				NewHireDate = DateTime.Today; NewDateOfBirth = null;
				NewStreet = string.Empty; NewPostalCode = string.Empty; NewCity = string.Empty; NewNotes = string.Empty;
				ErrorMessage = string.Empty;

				var addPage = _serviceProvider.GetRequiredService<Views.Employees.AddEmployeePage>();
				addPage.BindingContext = this;
				await window.Page.Navigation.PushAsync(addPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToAddEmployeeAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task SaveEmployeeAsync()
		{
			try
			{
				ErrorMessage = string.Empty;
				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				if (string.IsNullOrWhiteSpace(NewFirstName) || string.IsNullOrWhiteSpace(NewLastName) || string.IsNullOrWhiteSpace(NewEmail))
				{
					ErrorMessage = "Voornaam, Achternaam en E-mail zijn verplicht.";
					return;
				}

				decimal.TryParse(NewHourlyWage, out decimal wage);

				var dto = new EmployeeRequestDto
				{
					FirstName = NewFirstName,
					LastName = NewLastName,
					Email = NewEmail,
					EmployeeNumber = NewEmployeeNumber,
					JobTitle = NewJobTitle,
					HourlyWage = wage,
					HireDate = NewHireDate,
					DateOfBirth = NewDateOfBirth,
					Street = NewStreet,
					PostalCode = NewPostalCode,
					City = NewCity,
					Notes = NewNotes
				};

				bool success = await _apiService.CreateEmployeeAsync(dto);

				if (success)
				{
					await window.Page.Navigation.PopAsync();
					await LoadEmployeesAsync();
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Opslaan mislukt.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in SaveEmployeeAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToDetailsAsync(EmployeeDto selected)
		{
			try
			{
				if (selected == null) return;
				SelectedEmployee = selected;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var detailsPage = _serviceProvider.GetRequiredService<Views.Employees.EmployeeDetailsPage>();
				detailsPage.BindingContext = this;
				await window.Page.Navigation.PushAsync(detailsPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToDetailsAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task NavigateToEditEmployeeAsync()
		{
			try
			{
				if (SelectedEmployee == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				var editPage = _serviceProvider.GetRequiredService<Views.Employees.EditEmployeePage>();
				editPage.BindingContext = this;
				await window.Page.Navigation.PushAsync(editPage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in NavigateToEditEmployeeAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task UpdateEmployeeAsync()
		{
			try
			{
				ErrorMessage = string.Empty;
				if (SelectedEmployee == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				if (string.IsNullOrWhiteSpace(SelectedEmployee.FirstName) || string.IsNullOrWhiteSpace(SelectedEmployee.Email))
				{
					ErrorMessage = "Naam en e-mail zijn verplicht.";
					return;
				}

				var requestDto = new EmployeeRequestDto
				{
					FirstName = SelectedEmployee.FirstName,
					LastName = SelectedEmployee.LastName,
					Email = SelectedEmployee.Email,
					EmployeeNumber = SelectedEmployee.EmployeeNumber,
					JobTitle = SelectedEmployee.JobTitle,
					HourlyWage = SelectedEmployee.HourlyWage,
					HireDate = SelectedEmployee.HireDate,
					DateOfBirth = SelectedEmployee.DateOfBirth,
					Street = SelectedEmployee.Street,
					PostalCode = SelectedEmployee.PostalCode,
					City = SelectedEmployee.City,
					Notes = SelectedEmployee.Notes
				};

				bool isSuccess = await _apiService.UpdateEmployeeAsync(SelectedEmployee.Id, requestDto);

				if (isSuccess)
				{
					await window.Page.Navigation.PopAsync();
					int currentId = SelectedEmployee.Id;
					await LoadEmployeesAsync();
					SelectedEmployee = Employees.FirstOrDefault(e => e.Id == currentId);
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Werknemer kon niet worden aangepast.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in UpdateEmployeeAsync(): {ex}");
			}
		}

		[RelayCommand]
		public async Task DeleteEmployeeAsync()
		{
			try
			{
				if (SelectedEmployee == null) return;

				var window = Application.Current?.Windows.FirstOrDefault();
				if (window?.Page == null) return;

				bool isConfirmed = await window.Page.DisplayAlertAsync("Verwijderen", $"Weet je zeker dat je {SelectedEmployee.FirstName} wilt verwijderen?", "Ja", "Nee");
				if (!isConfirmed) return;

				bool isSuccess = await _apiService.DeleteEmployeeAsync(SelectedEmployee.Id);

				if (isSuccess)
				{
					await window.Page.Navigation.PopAsync();
					SelectedEmployee = null;
					await LoadEmployeesAsync();
				}
				else
				{
					await window.Page.DisplayAlertAsync("Fout", "Werknemer kon niet worden verwijderd.", "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in DeleteEmployeeAsync(): {ex}");
			}
		}
	}
}