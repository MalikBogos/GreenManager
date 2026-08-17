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
		private readonly IDbContextFactory<GreenManagerDbContext> _contextFactory;

		public ObservableCollection<Employee> Employees { get; set; }

		[ObservableProperty]
		private Employee _selectedEmployee;

		public EmployeeViewModel(IDbContextFactory<GreenManagerDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
			Employees = new ObservableCollection<Employee>();
			LoadEmployees();
		}

		private void LoadEmployees()
		{
			using (var context = _contextFactory.CreateDbContext()) { 
				var query = context.Employees
					.Include(e => e.User)
					.Where(e => !e.IsDeleted)
					.AsQueryable();

				var employeesFromDb = query.ToList();

				Employees.Clear();
				foreach (var employee in employeesFromDb)
				{
					Employees.Add(employee);
				}
			}
		}

		[RelayCommand]
		private void OpenAddWindow()
		{
			string newEmployeeNumber = "EMP001";

			using (var context = _contextFactory.CreateDbContext())
			{
				var lastEmployee = context.Employees.OrderByDescending(e => e.Id).FirstOrDefault();

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
				using (var context = _contextFactory.CreateDbContext())
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

			using (var context = _contextFactory.CreateDbContext())
			{
				var availableRoles = context.Roles.ToList();
				var currentRoleMap = context.UserRoles.FirstOrDefault(ur => ur.UserId == SelectedEmployee.ApplicationUserId);
				string currentRoleId = currentRoleMap != null ? currentRoleMap.RoleId : null;

				var editWindow = new EditEmployeeWindow(SelectedEmployee, availableRoles, currentRoleId);

				if (editWindow.ShowDialog() == true)
				{
					var userToUpdate = context.Users.Find(editWindow.EditedEmployee.ApplicationUserId);
					if (userToUpdate != null)
					{
						userToUpdate.FirstName = editWindow.EditedFirstName;
						userToUpdate.LastName = editWindow.EditedLastName;
						userToUpdate.PhoneNumber = editWindow.EditedPhoneNumber;
						context.Users.Update(userToUpdate);
					}

					if (editWindow.SelectedRoleId != null)
					{
						var existingRole = context.UserRoles.FirstOrDefault(ur => ur.UserId == editWindow.EditedEmployee.ApplicationUserId);

						if (existingRole != null && existingRole.RoleId != editWindow.SelectedRoleId)
						{
							context.UserRoles.Remove(existingRole);
							context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { UserId = editWindow.EditedEmployee.ApplicationUserId, RoleId = editWindow.SelectedRoleId });
						}
						else if (existingRole == null)
						{
							context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { UserId = editWindow.EditedEmployee.ApplicationUserId, RoleId = editWindow.SelectedRoleId });
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

			var result = MessageBox.Show($"Weet je zeker dat je werknemer '{SelectedEmployee.EmployeeNumber}' wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = _contextFactory.CreateDbContext())
				{
					var employeeToDelete = context.Employees.Include(e => e.User).FirstOrDefault(e => e.Id == SelectedEmployee.Id);

					if (employeeToDelete != null)
					{
						employeeToDelete.IsDeleted = true;
						employeeToDelete.DeletedAt = DateTime.UtcNow;
						employeeToDelete.DeletedReason = "Verwijderd via werknemersoverzicht";

						if (employeeToDelete.User != null)
						{
							employeeToDelete.User.IsDeleted = true;
							employeeToDelete.User.DeletedAt = DateTime.UtcNow;
							employeeToDelete.User.DeletedReason = "Gekoppelde werknemer verwijderd";
						}

						context.Employees.Update(employeeToDelete);
						context.SaveChanges();
					}
				}

				LoadEmployees();
			}
		}
	}
}
