using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager___WPF.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Data;
using Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GreenManager___WPF.ViewModels
{
	public partial class RegisterViewModel : ObservableObject
	{
		private readonly IDbContextFactory<GreenManagerDbContext> _contextFactory;
		private readonly UserManager<ApplicationUser> _userManager;


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

		public RegisterViewModel(IDbContextFactory<GreenManagerDbContext> contextFactory, UserManager<ApplicationUser> userManager)
		{
			_contextFactory = contextFactory;
			_userManager = userManager;
		}

		[RelayCommand]
		public async Task Register()
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

			var result = await _userManager.CreateAsync(newUser, Password);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(newUser, "Guest");

				using (var context = _contextFactory.CreateDbContext())
				{
					string newEmployeeNumber = "EMP001";
					var lastEmployee = context.Employees.AsNoTracking().OrderByDescending(e => e.Id).FirstOrDefault();

					if (lastEmployee != null && lastEmployee.EmployeeNumber != null)
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
						JobTitle = "Gast account",
						HireDate = DateTime.Today,
						CreatedAt = DateTime.UtcNow
					};

					context.Employees.Add(newEmployee);
					context.SaveChanges();
				}

				MessageBox.Show("Account succesvol aangemaakt! Je kan nu inloggen.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

				var loginWindow = App.AppHost.Services.GetRequiredService<LoginWindow>();
				loginWindow.Show();

				foreach (Window window in Application.Current.Windows)
				{
					if (window.DataContext == this)
					{
						window.Close();
						break;
					}
				}
			} else
			{
				var errorList = result.Errors.Select(e => e.Description);
				ErrorMessage = string.Join("\n", errorList);
			}
		}
	}
}