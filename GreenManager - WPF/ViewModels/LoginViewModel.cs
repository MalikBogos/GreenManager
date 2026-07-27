using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager___WPF.Views;
using Microsoft.AspNetCore.Identity;
using Models.Data;
using Models.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GreenManager___WPF.ViewModels
{
	public partial class LoginViewModel : ObservableObject
	{
		// Automatically turns into EmailInput
		[ObservableProperty]
		private string _emailInput;

		[ObservableProperty]
		private string _errorMessage;

		// Automatically turns into LoginCommand
		[RelayCommand]
		private void Login(object parameter)
		{
			ErrorMessage = string.Empty;

			var passwordBox = parameter as PasswordBox;
			var password = passwordBox?.Password;

			if (string.IsNullOrWhiteSpace(EmailInput) || string.IsNullOrWhiteSpace(password))
			{
				ErrorMessage = "Vul a.u.b. zowel een e-mailadres als een wachtwoord in.";
				return;
			}

			using (var context = new GreenManagerDbContext())
			{
				var user = context.Users.FirstOrDefault(u => u.Email == EmailInput);

				if (user == null)
				{
					ErrorMessage = "E-mailadres of wachtwoord is onjuist.";
					return;
				}

				var hasher = new PasswordHasher<ApplicationUser>();
				var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

				if (result == PasswordVerificationResult.Success)
				{
					MessageBox.Show($"Welkom, {user.FirstName}!", "Succesvol Ingelogd", MessageBoxButton.OK, MessageBoxImage.Information);

					var mainWindow = new MainWindow(user);
					mainWindow.Show();

					foreach (Window window in Application.Current.Windows)
					{
						if (window.DataContext == this)
						{
							window.Close();
							break;
						}
					}
				}
				else
				{
					ErrorMessage = "E-mailadres of wachtwoord is onjuist.";
				}
			}
		}
	}
}