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
	/// Interaction logic for ProjectDetailWindow.xaml
	/// </summary>
	public partial class ProjectDetailWindow : Window
	{
		public Project EditedProject { get; set; }
		public List<Customer> AvailableCustomers { get; set; }

		// Geef de mogelijke enums weer
		public List<ProjectStatus> AvailableStatuses { get; set; }

		public ProjectDetailWindow(Project selectedProject, List<Customer> customers)
		{
			InitializeComponent();

			AvailableCustomers = customers;
			AvailableStatuses = Enum.GetValues(typeof(ProjectStatus)).Cast<ProjectStatus>().ToList();

			EditedProject = new Project
			{
				Id = selectedProject.Id,
				Name = selectedProject.Name,
				CustomerId = selectedProject.CustomerId,
				Status = selectedProject.Status,
				StartDate = selectedProject.StartDate,
				EndDate = selectedProject.EndDate,
				ProjectAddress = selectedProject.ProjectAddress,
				Budget = selectedProject.Budget,
				Description = selectedProject.Description,
				Notes = selectedProject.Notes,
				CreatedAt = selectedProject.CreatedAt
			};

			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(EditedProject.Name) || EditedProject.CustomerId == 0)
			{
				MessageBox.Show("Projectnaam en Klant zijn verplicht.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
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
