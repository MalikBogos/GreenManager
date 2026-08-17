using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GreenManager___WPF.ViewModels;

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for RegisterWindow.xaml
	/// </summary>
	public partial class RegisterWindow : Window
	{
		public RegisterWindow(RegisterViewModel viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
		}

		private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (this.DataContext is RegisterViewModel viewModel)
			{
				viewModel.Password = TxtPassword.Password;
			}
		}

		private void TxtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (this.DataContext is RegisterViewModel viewModel)
			{
				viewModel.ConfirmPassword = TxtConfirmPassword.Password;
			}
		}

	}
}
