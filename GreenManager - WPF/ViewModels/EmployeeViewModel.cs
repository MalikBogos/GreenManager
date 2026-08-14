using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using GreenManager___WPF.Views;
using Microsoft.EntityFrameworkCore;

namespace GreenManager___WPF.ViewModels
{
	public partial class EmployeeViewModel : ObservableObject
	{
		public ObservableCollection<Employee> Employees { get; set; }

		[ObservableProperty]
		private Employee _selectedEmployee;

		// Property to bind to the CheckBox in the UI
		[ObservableProperty]
		private bool _showDeleted;

		// This built-in CommunityToolkit method triggers automatically whenever ShowDeleted changes # notes
		partial void OnShowDeletedChanged(bool value)
		{
			LoadEmployees();
		}

		public EmployeeViewModel()
		{
			Employees = new ObservableCollection<Employee>();
			LoadEmployees();
		}

		private void LoadEmployees()
		{
			using (var context = new GreenManagerDbContext())
			{
				var query = context.Employees
					.Include(e => e.Addresses)
					.Include(e => e.WageHistory)
					.AsQueryable();

				if (!ShowDeleted)
				{
					query = query.Where(e => e.IsDeleted == false);
				}

				var EmployeesFromDb = query.ToList();
				Employees.Clear();

				foreach (var employee in EmployeesFromDb)
				{
					Employees.Add(employee);
				}
			}
		}

		[RelayCommand]
		private void OpenAddWindow()
		{
			string newEmployeeNumber = "EMP001";

			using (var dbContext = new GreenManagerDbContext())
			{
				var lastEmployee = dbContext.Employees.OrderByDescending(e => e.Id).FirstOrDefault();

				if (lastEmployee != null && lastEmployee.EmployeeNumber != null && lastEmployee.EmployeeNumber.StartsWith("EMP"))
				{
					string numberPart = lastEmployee.EmployeeNumber.Substring(3);

					if (int.TryParse(numberPart, out int lastNumber))
					{
						newEmployeeNumber = $"EMP{(lastNumber + 1):D3}";
					}
				}
			}

			var addWindow = new AddEmployeeWindow(newEmployeeNumber);

			if (addWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					var newLoginAccount = new ApplicationUser
					{
						Id = Guid.NewGuid().ToString(),
						UserName = addWindow.UserEmail,
						NormalizedUserName = addWindow.UserEmail.ToUpper(),
						Email = addWindow.UserEmail,
						NormalizedEmail = addWindow.UserEmail.ToUpper(),
						FirstName = addWindow.UserFirstName,
						LastName = addWindow.UserLastName,
						EmailConfirmed = true,
						CreatedAt = DateTime.UtcNow
					};

					var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();
					newLoginAccount.PasswordHash = hasher.HashPassword(newLoginAccount, "Welcome123!");

					context.Users.Add(newLoginAccount);

					addWindow.NewEmployee.ApplicationUserId = newLoginAccount.Id;

					context.Employees.Add(addWindow.NewEmployee);
					context.SaveChanges();
				}

				LoadEmployees();
			}
		}

		[RelayCommand]
		private void EditEmployee()
		{
			if (SelectedEmployee == null)
			{
				MessageBox.Show("Selecteer eerst een werknemer om te bewerken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			if (SelectedEmployee.IsDeleted)
			{
				MessageBox.Show("Dit dossier is verplaatst naar het archief en kan niet meer bewerkt worden.", "Actie niet toegestaan", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var editWindow = new EditEmployeeWindow(SelectedEmployee);
			if (editWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					context.Employees.Update(editWindow.EditedEmployee);

					if (!string.IsNullOrWhiteSpace(editWindow.EditedAddress.AddressLine1))
					{
						if (editWindow.EditedAddress.Id == 0)
						{
							context.Addresses.Add(editWindow.EditedAddress);
						}
						else
						{
							var origineelAdres = context.Addresses.AsNoTracking().FirstOrDefault(a => a.Id == editWindow.EditedAddress.Id);

							if (origineelAdres != null &&
								(origineelAdres.AddressLine1 != editWindow.EditedAddress.AddressLine1 ||
								 origineelAdres.PostalCode != editWindow.EditedAddress.PostalCode ||
								 origineelAdres.City != editWindow.EditedAddress.City))
							{

								var teArchiverenAdres = context.Addresses.Find(origineelAdres.Id);
								if (teArchiverenAdres != null)
								{
									teArchiverenAdres.IsDeleted = true;
									teArchiverenAdres.DeletedAt = DateTime.UtcNow;
									teArchiverenAdres.DeletedReason = "Verhuizing naar nieuw adres";
									context.Addresses.Update(teArchiverenAdres);
								}

								var nieuwAdresVoorVerhuizing = new Address
								{
									EmployeeId = editWindow.EditedAddress.EmployeeId,
									AddressLine1 = editWindow.EditedAddress.AddressLine1,
									PostalCode = editWindow.EditedAddress.PostalCode,
									City = editWindow.EditedAddress.City,
									Country = editWindow.EditedAddress.Country,
									AddressType = editWindow.EditedAddress.AddressType
								};

								context.Addresses.Add(nieuwAdresVoorVerhuizing);
							}
							else
							{
								context.Addresses.Update(editWindow.EditedAddress);
							}
						}
					}
					editWindow.EditedEmployee.UpdatedAt = DateTime.UtcNow;

					context.Employees.Update(editWindow.EditedEmployee);
					context.SaveChanges();
				}
				LoadEmployees();
			}
		}

		[RelayCommand]
		private void DeleteEmployee()
		{
			if (SelectedEmployee == null)
			{
				MessageBox.Show("Selecteer eerst een werknemer om te verwijderen.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (SelectedEmployee.IsDeleted)
			{
				MessageBox.Show("Deze werknemer is al verwijderd.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var result = MessageBox.Show($"Weet je zeker dat je werknemer '{SelectedEmployee.EmployeeNumber}' wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					var employeeToDelete = context.Employees.Find(SelectedEmployee.Id);

					if (employeeToDelete != null)
					{
						employeeToDelete.IsDeleted = true;
						employeeToDelete.DeletedAt = DateTime.UtcNow;
						employeeToDelete.DeletedReason = "Verwijderd door de Admin via het werknemersoverzicht";

						context.Employees.Update(employeeToDelete);
						context.SaveChanges();
					}
				}

				LoadEmployees();
			}
		}

		[RelayCommand]
		private void ManageWages()
		{
			if (SelectedEmployee == null)
			{
				MessageBox.Show("Selecteer eerst een werknemer om de lonen te beheren.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			if (SelectedEmployee.IsDeleted)
			{
				MessageBox.Show("Je kunt geen lonen toevoegen aan een gearchiveerd dossier.", "Niet toegestaan", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Open the new wage management window
			var wageWindow = new WageHistoryWindow(SelectedEmployee);

			// If the window closes and a wage was added, reload to update the CurrentHourlyWage column
			if (wageWindow.ShowDialog() == true)
			{
				LoadEmployees();
			}
		}
	}
}
