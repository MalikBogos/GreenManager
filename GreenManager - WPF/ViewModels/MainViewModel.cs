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

		public string WelcomeMessage => $"Welkom, {CurrentUserRole} {CurrentUser?.FirstName}!";

		public MainViewModel(ApplicationUser user, string roleName)
		{
			CurrentUser = user;
			CurrentUserRole = roleName;
		}	
	}

}