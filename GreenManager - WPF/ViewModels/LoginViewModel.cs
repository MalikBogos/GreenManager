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
	// 1. De klasse MOET 'partial' zijn en overerven van 'ObservableObject'
	public partial class LoginViewModel : ObservableObject
	{
		// 2. Gebruik [ObservableProperty] op velden met een kleine letter. 
		// De toolkit maakt hier op de achtergrond automatisch EmailInput (hoofdletter) van voor je XAML!
		[ObservableProperty]
		private string _emailInput;

		[ObservableProperty]
		private string _errorMessage;

		// Geen constructor meer nodig om commands te koppelen!

		// 3. Gebruik [RelayCommand] op je methode. 
		// De toolkit maakt hier automatisch 'LoginCommand' van voor in je XAML.
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

					var mainWindow = new MainWindow();
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