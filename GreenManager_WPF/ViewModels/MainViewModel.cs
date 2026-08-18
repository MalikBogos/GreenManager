using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GreenManager_WPF.ViewModels
{

	public partial class MainViewModel : ObservableObject
	{
		[ObservableProperty]
		private ApplicationUser _currentUser;

		[ObservableProperty]
		private string _currentUserRole;

		// Deze is WAAR (true) als de rol 'Admin' is. Anders is hij onwaar (false).
		public bool IsAdmin => CurrentUserRole == "Admin";

		// Deze is WAAR (true) als de gebruiker GEEN gast is (dus Admin of Werknemer mag dit zien).
		public bool IsNotGuest => CurrentUserRole != "Guest";

		public string WelcomeMessage => $"Welkom, {CurrentUserRole} {CurrentUser.FirstName}!";

		public MainViewModel()
		{
		}

		public void SetupUser(ApplicationUser user, string roleName)
		{
			CurrentUser = user;
			CurrentUserRole = roleName;
		}

		[RelayCommand]
		public void Logout()
		{
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
		}
	}

}