using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Identity;
using Models.Data;
using Models.Entities;
using System;
using System.Linq;
using System.Windows;

namespace GreenManager___WPF.ViewModels
{
	public partial class RegisterViewModel : ObservableObject
	{
		[ObservableProperty]
		private string _firstName = string.Empty;

		[ObservableProperty]
		private string _lastName = string.Empty;

		[ObservableProperty]
		private string _email = string.Empty;

		[ObservableProperty]
		private string _password = string.Empty;

		[ObservableProperty]
		private string _confirmPassword = string.Empty;

		[ObservableProperty]
		private string _errorMessage = string.Empty;

		[RelayCommand]
		public void Register()
		{
			ErrorMessage = string.Empty;

			if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
				string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
			{
				ErrorMessage = "Vul a.u.b. alle velden in.";
				return;
			}

			if (Password != ConfirmPassword)
			{
				ErrorMessage = "Wachtwoorden komen niet overeen.";
				return;
			}

			using (var db = new GreenManagerDbContext())
			{
				if (db.Users.Any(u => u.Email == Email))
				{
					ErrorMessage = "Dit e-mailadres is al in gebruik.";
					return;
				}

				var newUser = new ApplicationUser
				{
					Id = Guid.NewGuid().ToString(),
					UserName = Email,
					NormalizedUserName = Email.ToUpper(),
					Email = Email,
					NormalizedEmail = Email.ToUpper(),
					FirstName = this.FirstName, 
					LastName = this.LastName,   
					EmailConfirmed = true,
					CreatedAt = DateTime.UtcNow
				};

				var hasher = new PasswordHasher<ApplicationUser>();
				newUser.PasswordHash = hasher.HashPassword(newUser, Password);

				db.Users.Add(newUser);

				var guestRoleMapping = new IdentityUserRole<string>
				{
					RoleId = "role-guest-3",
					UserId = newUser.Id
				};
				db.UserRoles.Add(guestRoleMapping);

				string newEmployeeNumber = "EMP001";
				var lastEmployee = db.Employees.OrderByDescending(e => e.Id).FirstOrDefault();

				if (lastEmployee != null && lastEmployee.EmployeeNumber != null && lastEmployee.EmployeeNumber.StartsWith("EMP"))
				{
					string numberPart = lastEmployee.EmployeeNumber.Substring(3);
					if (int.TryParse(numberPart, out int lastNumber))
					{
						newEmployeeNumber = $"EMP{(lastNumber + 1):D3}";
					}
				}

				var newEmployee = new Employee
				{
					ApplicationUserId = newUser.Id,
					EmployeeNumber = newEmployeeNumber,
					JobTitle = "Gast / Nieuw Account",
					HireDate = DateTime.Today,
					CreatedAt = DateTime.UtcNow
				};
				db.Employees.Add(newEmployee);

				db.SaveChanges();

				MessageBox.Show("Account succesvol aangemaakt! Je kunt nu inloggen.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

				foreach (Window window in Application.Current.Windows)
				{
					if (window.DataContext == this)
					{
						window.Close();
						break;
					}
				}
			}
		}
	}
}