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

		public AddEmployeeWindow(string generatedNumber)
		{
			InitializeComponent();

			NewEmployee = new Employee
			{
				ApplicationUserId = "wordt-zo-ingevuld",
				EmployeeNumber = generatedNumber,
				JobTitle = "",
				HireDate = DateTime.Today,
				CreatedAt = DateTime.UtcNow
			};

			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(NewEmployee.EmployeeNumber) || string.IsNullOrWhiteSpace(NewEmployee.JobTitle))
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
