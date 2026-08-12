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

		public EditEmployeeWindow(Employee selectedEmployee)
		{
			InitializeComponent();

			EditedEmployee = new Employee
			{
				Id = selectedEmployee.Id,
				ApplicationUserId = selectedEmployee.ApplicationUserId,
				EmployeeNumber = selectedEmployee.EmployeeNumber,
				JobTitle = selectedEmployee.JobTitle,
				HireDate = selectedEmployee.HireDate,
				CreatedAt = selectedEmployee.CreatedAt
			};

			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(EditedEmployee.EmployeeNumber) || string.IsNullOrWhiteSpace(EditedEmployee.JobTitle))
			{
				MessageBox.Show("Vul de verplichte velden (Personeelsnummer en Functie) in.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
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
