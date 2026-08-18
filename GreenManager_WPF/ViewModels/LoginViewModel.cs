using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_WPF.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Data;
using Models.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GreenManager_WPF.ViewModels
{
	public partial class LoginViewModel : ObservableObject
	{
		private readonly UserManager<ApplicationUser> _userManager;

		// Automatically turns into EmailInput
		[ObservableProperty]
		private string _emailInput;

		[ObservableProperty]
		private string _errorMessage;

		public LoginViewModel(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		[RelayCommand]
		private async Task Login(object parameter)
		{
			ErrorMessage = string.Empty;

			var passwordBox = parameter as PasswordBox;
			var password = passwordBox?.Password;

			if (string.IsNullOrWhiteSpace(EmailInput) || string.IsNullOrWhiteSpace(password))
			{
				ErrorMessage = "Vul a.u.b. zowel een e-mailadres als een wachtwoord in.";
				return;
			}

			var user = await _userManager.FindByEmailAsync(EmailInput);

			if (user == null || user.IsDeleted)
			{
				ErrorMessage = "E-mailadres of wachtwoord is onjuist, of dit account bestaat niet (meer)";
				return;
			}

			bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);

			if (isPasswordCorrect)
			{
				var roles = await _userManager.GetRolesAsync(user);
				string roleName = roles.Count > 0 ? roles[0] : "Onbekend";

				var mainWindow = App.AppHost.Services.GetRequiredService<MainWindow>();

				if (mainWindow.DataContext is MainViewModel mainVM)
				{
					mainVM.SetupUser(user, roleName);
				}
				mainWindow.Show();

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
				ErrorMessage = "Emailadres of wachtwoord is niet juist.";
			}
			
		}

		[RelayCommand]
		private void Register(object parameter)
		{
			var registerWindow = App.AppHost.Services.GetRequiredService<RegisterWindow>();

			registerWindow.Show();

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