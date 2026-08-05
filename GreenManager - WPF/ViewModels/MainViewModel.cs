using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GreenManager___WPF.ViewModels
{

	public partial class MainViewModel : ObservableObject
	{
		[ObservableProperty]
		private ApplicationUser _currentUser;

		[ObservableProperty]
		private string _currentUserRole;

		// Deze is WAAR (true) als de rol 'Admin' is. Anders is hij onwaar (false).
		public bool IsAdmin => CurrentUserRole == "Admin";

		// Deze is WAAR (true) als de gebruiker GEEN gast is (dus Admin óf Werknemer mag dit zien).
		public bool IsNotGuest => CurrentUserRole != "Guest";

		public string WelcomeMessage => $"Welkom, {CurrentUserRole} {CurrentUser?.FirstName}!";

		public MainViewModel(ApplicationUser user, string roleName)
		{
			CurrentUser = user;
			CurrentUserRole = roleName;
		}	
	}

}