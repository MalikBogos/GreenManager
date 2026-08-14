using Models.Entities;
using Models.Enums;
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
	/// Interaction logic for AddProjectWindow.xaml
	/// </summary>
	public partial class AddProjectWindow : Window
	{
		// Nieuw project data
		public Project NewProject { get; set; }

		public List<Customer> AvailableCustomers { get; set; }

		public AddProjectWindow(List<Customer> customers)
		{
			InitializeComponent();

			AvailableCustomers = customers;

			// Prepare a fresh project object
			NewProject = new Project
			{
				Name = "",
				StartDate = DateTime.Today,
				Status = ProjectStatus.Quotation, // Project wordt aangemaakt als een quotation
				CreatedAt = DateTime.UtcNow
			};

			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(NewProject.Name) || NewProject.CustomerId == 0)
			{
				MessageBox.Show("Vul een projectnaam in en selecteer een klant.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
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
