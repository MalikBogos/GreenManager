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
		
		public EmployeeViewModel()
		{
			Employees = new ObservableCollection<Employee>();
			LoadEmployees();
		}

		private void LoadEmployees()
		{
			using (var context = new GreenManagerDbContext())
			{
				var EmployeesFromDb = context.Employees.Include(e => e.Addresses).ToList();
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
			var addWindow = new AddEmployeeWindow();

			if (addWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					var newLoginAccount = new ApplicationUser
					{
						Id = Guid.NewGuid().ToString(),
						UserName = $"{addWindow.NewEmployee.EmployeeNumber}@greenmanager.be",
						NormalizedUserName = $"{addWindow.NewEmployee.EmployeeNumber}@GREENMANAGER.BE",
						Email = $"{addWindow.NewEmployee.EmployeeNumber}@greenmanager.be",
						NormalizedEmail = $"{addWindow.NewEmployee.EmployeeNumber}@GREENMANAGER.BE",
						FirstName = "Nieuwe",
						LastName = "Werknemer",
						EmailConfirmed = true,
						CreatedAt = DateTime.UtcNow
					};

					var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();
					newLoginAccount.PasswordHash = hasher.HashPassword(newLoginAccount, "Welkom123!");

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

			 var editWindow = new EditEmployeeWindow(SelectedEmployee);
			if (editWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					// 1. Update de werknemer zelf (Functie, etc.)
					context.Employees.Update(editWindow.EditedEmployee);

					// 2. Slimme Adres Logica
					if (!string.IsNullOrWhiteSpace(editWindow.EditedAddress.AddressLine1))
					{
						if (editWindow.EditedAddress.Id == 0)
						{
							// 2A: Het is een compleet nieuw adres voor iemand die er nog geen had
							context.Addresses.Add(editWindow.EditedAddress);
						}
						else
						{
							// 2B: We halen het originele adres zonder wijzigingen uit de database om te vergelijken
							var origineelAdres = context.Addresses.AsNoTracking().FirstOrDefault(a => a.Id == editWindow.EditedAddress.Id);

							// We checken of er een "echte" wijziging is gedaan (een verhuizing)
							if (origineelAdres != null &&
								(origineelAdres.AddressLine1 != editWindow.EditedAddress.AddressLine1 ||
								 origineelAdres.PostalCode != editWindow.EditedAddress.PostalCode ||
								 origineelAdres.City != editWindow.EditedAddress.City))
							{
								// Er is een verhuizing! We archiveren de oude en maken een nieuwe.

								// Stap 1: Oud adres op 'Verwijderd' zetten (geschiedenis bewaren)
								var teArchiverenAdres = context.Addresses.Find(origineelAdres.Id);
								if (teArchiverenAdres != null)
								{
									teArchiverenAdres.IsDeleted = true;
									teArchiverenAdres.DeletedAt = DateTime.UtcNow;
									teArchiverenAdres.DeletedReason = "Verhuizing naar nieuw adres";
									context.Addresses.Update(teArchiverenAdres);
								}

								// Stap 2: Het aangepaste formulier opslaan als GLOEDNIEUW adres
								// OMDAT we het Id niet mogen aanpassen (init-only), maken we een heel nieuw object aan:
								var nieuwAdresVoorVerhuizing = new Address
								{
									// Id hoeven we niet in te vullen, dat is standaard al 0 voor een nieuw object!
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
								// Er is niets wezenlijks veranderd (hooguit een typfout of helemaal niets)
								// We overschrijven (updaten) de huidige regel gewoon.
								context.Addresses.Update(editWindow.EditedAddress);
							}
						}
					}

					// 3. Sla alles veilig op in de database
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
				MessageBox.Show("Selecteer eerst een werknemer om te verwijderen.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Weet je zeker dat je werknemer '{SelectedEmployee.EmployeeNumber}' wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					SelectedEmployee.IsDeleted = true;
					context.Employees.Update(SelectedEmployee);
					context.SaveChanges();
				}
				LoadEmployees();
			}
		}
	}
}
