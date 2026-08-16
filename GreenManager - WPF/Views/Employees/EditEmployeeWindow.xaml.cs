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

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for EditEmployeeWindow.xaml
	/// </summary>
	public partial class EditEmployeeWindow : Window
	{
		public Employee EditedEmployee { get; set; }
		public Address EditedAddress { get; set; }

		// NIEUW: Eigenschappen voor de voornaam en achternaam
		public string EditedFirstName { get; set; } = string.Empty;
		public string EditedLastName { get; set; } = string.Empty;

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
			}

			EditedEmployee = new Employee
			{
				Id = selectedEmployee.Id,
				ApplicationUserId = selectedEmployee.ApplicationUserId,
				EmployeeNumber = selectedEmployee.EmployeeNumber,
				JobTitle = selectedEmployee.JobTitle,
				HireDate = selectedEmployee.HireDate,
				CreatedAt = selectedEmployee.CreatedAt,

				Department = selectedEmployee.Department,
				DateOfBirth = selectedEmployee.DateOfBirth,
				EmergencyContactName = selectedEmployee.EmergencyContactName,
				EmergencyContactPhone = selectedEmployee.EmergencyContactPhone,
				Notes = selectedEmployee.Notes
			};

			var currentAddress = selectedEmployee.CurrentAddress;

			if (currentAddress != null)
			{
				EditedAddress = new Address
				{
					Id = currentAddress.Id,
					EmployeeId = currentAddress.EmployeeId,
					AddressLine1 = currentAddress.AddressLine1,
					AddressLine2 = currentAddress.AddressLine2,
					PostalCode = currentAddress.PostalCode,
					Province = currentAddress.Province,
					City = currentAddress.City,
					Country = currentAddress.Country
				};
			}
			else
			{
				EditedAddress = new Address
				{
					EmployeeId = selectedEmployee.Id,
					AddressLine1 = "",
					AddressLine2 = "",
					PostalCode = "",
					Province = "",
					City = "",
					Country = "Belgium"
				};
			}

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
