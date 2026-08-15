using Models.Entities;
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

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for AddEmployeeWindow.xaml
	/// </summary>
	public partial class AddEmployeeWindow : Window
	{
		public Employee NewEmployee { get; set; }

		public string UserFirstName { get; set; } = string.Empty;
		public string UserLastName { get; set; } = string.Empty;
		public string UserEmail { get; set; } = string.Empty;

		public AddEmployeeWindow(string generatedNumber)
		{
			InitializeComponent();

			NewEmployee = new Employee
			{
				ApplicationUserId = "will-be-filled-in-later",
				EmployeeNumber = generatedNumber,
				JobTitle = "",
				HireDate = DateTime.Today,
				CreatedAt = DateTime.UtcNow
			};

			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(NewEmployee.EmployeeNumber) ||
				string.IsNullOrWhiteSpace(NewEmployee.JobTitle) ||
				string.IsNullOrWhiteSpace(UserFirstName) ||
				string.IsNullOrWhiteSpace(UserLastName) ||
				string.IsNullOrWhiteSpace(UserEmail))
			{
				MessageBox.Show("Vul a.u.b. alle verplichte velden in (inclusief naam en e-mail).", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			this.DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}
