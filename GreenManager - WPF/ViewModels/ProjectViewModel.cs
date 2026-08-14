using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager___WPF.Views;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace GreenManager___WPF.ViewModels
{
	public partial class ProjectViewModel : ObservableObject
	{
		public ObservableCollection<Project> Projects { get; set; }

		[ObservableProperty]
		private Project _selectedProject;

		public ProjectViewModel()
		{
			Projects = new ObservableCollection<Project>();

			LoadProjects();
		}

		private void LoadProjects()
		{
			using (var context = new GreenManagerDbContext())
			{
				var ProjectsFromDb = context.Projects.ToList();

				Projects.Clear();
				foreach(var project in ProjectsFromDb)
				{
					Projects.Add(project);
				}
			}
		}

		[RelayCommand]
		private void OpenAddProjectWindow()
		{
			using (var context = new GreenManagerDbContext())
			{
				// Haal alle klanten op om ze weer te geven volgens achternaam
				var activeCustomers = context.Customers
					.Where(c => c.IsDeleted == false)
					.OrderBy(c => c.LastName)
					.ToList();

				// Bevestig ervoor dat er minstens 1 klant bestaat
				if (activeCustomers.Count == 0)
				{
					MessageBox.Show("Voeg eerst een klant toe voordat je een project of offerte kunt aanmaken.", "Geen klanten gevonden", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// 2. Open een nieuw AddProjectWindow en geef activecustomers door
				var addWindow = new AddProjectWindow(activeCustomers);

				// 3. Sla het project op indien de gebruiker op Opslaan drukt (ShowDialog() == true)
				if (addWindow.ShowDialog() == true)
				{
					context.Projects.Add(addWindow.NewProject);
					context.SaveChanges();

					// Herlaad de lijst van projecten
					LoadProjects();
				}
			}
		}

		[RelayCommand]
		private void OpenProjectDetails()
		{
			if (SelectedProject == null)
			{
				MessageBox.Show("Selecteer eerst een project om de details te bekijken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			using (var context = new GreenManagerDbContext())
			{
				// Haal een lijst op van klanten die niet zijn verwijderd
				var activeCustomers = context.Customers.Where(c => c.IsDeleted == false).ToList();

				var detailWindow = new ProjectDetailWindow(SelectedProject, activeCustomers);

				if (detailWindow.ShowDialog() == true)
				{
					// Update the audit field
					detailWindow.EditedProject.UpdatedAt = DateTime.UtcNow;

					context.Projects.Update(detailWindow.EditedProject);
					context.SaveChanges();

					LoadProjects();
				}
			}
		}
	}
}
