using Microsoft.AspNetCore.Identity;
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

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for EditEmployeeWindow.xaml
	/// </summary>
	public partial class EditEmployeeWindow : Window
	{
		public Employee EditedEmployee { get; set; }

		public string EditedFirstName { get; set; } = string.Empty;
		public string EditedLastName { get; set; } = string.Empty;
		public string EditedPhoneNumber { get; set; } = string.Empty;

		public string SelectedRoleId { get; set; }
		public List<IdentityRole> AvailableRoles { get; set; }

		public EditEmployeeWindow(Employee selectedEmployee, List<IdentityRole> roles, string currentRoleId)
		{
			InitializeComponent();

			AvailableRoles = roles;
			SelectedRoleId = currentRoleId;

			if (selectedEmployee.User != null)
			{
				EditedFirstName = selectedEmployee.User.FirstName;
				EditedLastName = selectedEmployee.User.LastName;

				EditedPhoneNumber = selectedEmployee.User.PhoneNumber ?? string.Empty;
			}

			EditedEmployee = new Employee
			{
				Id = selectedEmployee.Id,
				ApplicationUserId = selectedEmployee.ApplicationUserId,
				EmployeeNumber = selectedEmployee.EmployeeNumber,
				JobTitle = selectedEmployee.JobTitle,
				HireDate = selectedEmployee.HireDate,
				CreatedAt = selectedEmployee.CreatedAt,
				DateOfBirth = selectedEmployee.DateOfBirth,
				Notes = selectedEmployee.Notes,
				Street = selectedEmployee.Street,
				PostalCode = selectedEmployee.PostalCode,
				City = selectedEmployee.City
			};
			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(EditedEmployee.EmployeeNumber) ||
				string.IsNullOrWhiteSpace(EditedEmployee.JobTitle) ||
				string.IsNullOrWhiteSpace(EditedFirstName) ||
				string.IsNullOrWhiteSpace(EditedLastName))
			{
				MessageBox.Show("Vul de verplichte velden (Voornaam, Achternaam, Personeelsnummer en Functie) in.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
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
