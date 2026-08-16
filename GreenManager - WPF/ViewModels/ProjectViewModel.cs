using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager___WPF.Views;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.Base;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace GreenManager___WPF.ViewModels
{
	public partial class ProjectViewModel : ObservableObject
	{
		#region --- 1. HOOFDSCHERM (OVERZICHT LIJST) ---

		public ObservableCollection<Project> Projects { get; set; }

		[ObservableProperty]
		private Project _selectedProject;

		public ProjectViewModel()
		{
			Projects = new ObservableCollection<Project>();
			ProjectEmployees = new ObservableCollection<ProjectEmployee>();
			ProjectMaterials = new ObservableCollection<ProjectMaterial>();
			WorkLogs = new ObservableCollection<WorkLog>();

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
				var activeCustomers = context.Customers.Where(c => c.IsDeleted == false).OrderBy(c => c.LastName).ToList();

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

		#endregion

		#region --- 2. BEWERKSCHERM (EDIT PROJECT STATE) ---

		[ObservableProperty] private Project _editedProject;
		[ObservableProperty] private List<Customer> _availableCustomers;
		[ObservableProperty] private List<ProjectStatus> _availableStatuses;
		[ObservableProperty] private List<Employee> _availableEmployees;
		[ObservableProperty] private List<Material> _availableMaterials;

		public ObservableCollection<ProjectEmployee> ProjectEmployees { get; set; }
		public ObservableCollection<ProjectMaterial> ProjectMaterials { get; set; }
		public ObservableCollection<WorkLog> WorkLogs { get; set; }

		// Inputs voor Tab 2: Planning
		[ObservableProperty] private int _selectedEmployeeId;
		[ObservableProperty] private DateTime _newPlannedDate = DateTime.Today;
		[ObservableProperty] private decimal _newEstimatedHours;

		// Inputs voor Tab 3: Materials
		[ObservableProperty] private int _selectedMaterialId;
		[ObservableProperty] private decimal _newQuantity;

		// Inputs voor Tab 4: WorkLogs
		[ObservableProperty] private int _selectedWorkEmployeeId;
		[ObservableProperty] private DateTime _newWorkDate = DateTime.Today;
		[ObservableProperty] private decimal _newHoursWorked;
		[ObservableProperty] private string _newTaskDescription = string.Empty;

		[RelayCommand]
		private void EditProject()
		{
			if (SelectedProject == null)
			{
				MessageBox.Show("Selecteer eerst een project om te bewerken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			// Laad dropdowns en bereid het project voor
			using (var context = new GreenManagerDbContext())
			{
				AvailableCustomers = context.Customers.Where(c => !c.IsDeleted).ToList();
				AvailableStatuses = Enum.GetValues(typeof(ProjectStatus)).Cast<ProjectStatus>().ToList();
			}

			EditedProject = new Project
			{
				Id = SelectedProject.Id,
				Name = SelectedProject.Name,
				CustomerId = SelectedProject.CustomerId,
				Status = SelectedProject.Status,
				StartDate = SelectedProject.StartDate,
				EndDate = SelectedProject.EndDate,
				ProjectAddress = SelectedProject.ProjectAddress,
				Budget = SelectedProject.Budget,
				Description = SelectedProject.Description,
				Notes = SelectedProject.Notes,
				CreatedAt = SelectedProject.CreatedAt
			};

			LoadTabData();

			// Open het nieuwe venster en geef DIT ViewModel mee
			var editWindow = new EditProjectWindow(this);
			if (editWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					EditedProject.UpdatedAt = DateTime.UtcNow;
					context.Projects.Update(EditedProject);
					context.SaveChanges();
				}
				LoadProjects();
			}
		}

		private void LoadTabData()
		{
			using (var context = new GreenManagerDbContext())
			{
				AvailableEmployees = context.Employees.Include(e => e.User).Where(e => !e.IsDeleted).OrderBy(e => e.User.LastName).ToList();
				AvailableMaterials = context.Materials.Where(m => !m.IsDeleted).OrderBy(m => m.Name).ToList();

				var pEmployees = context.Set<ProjectEmployee>().Include(pe => pe.Employee).ThenInclude(e => e.User).Where(pe => pe.ProjectId == EditedProject.Id && !pe.IsDeleted).ToList();
				ProjectEmployees.Clear();
				foreach (var pe in pEmployees) ProjectEmployees.Add(pe);

				var pMaterials = context.Set<ProjectMaterial>().Include(pm => pm.Material).Where(pm => pm.ProjectId == EditedProject.Id && !pm.IsDeleted).ToList();
				ProjectMaterials.Clear();
				foreach (var pm in pMaterials) ProjectMaterials.Add(pm);

				var wLogs = context.WorkLogs.Include(w => w.Employee).ThenInclude(e => e.User).Where(w => w.ProjectId == EditedProject.Id && !w.IsDeleted).ToList();
				WorkLogs.Clear();
				foreach (var w in wLogs) WorkLogs.Add(w);
			}
		}
		#endregion

		#region --- 3. TABBLAD COMMANDO'S ---

		[RelayCommand]
		private void AddEmployee()
		{
			if (SelectedEmployeeId == 0 || NewEstimatedHours <= 0) return;
			using (var context = new GreenManagerDbContext())
			{
				var newPlan = new ProjectEmployee { ProjectId = EditedProject.Id, EmployeeId = SelectedEmployeeId, PlannedDate = NewPlannedDate, EstimatedHours = NewEstimatedHours };
				context.Set<ProjectEmployee>().Add(newPlan);
				context.SaveChanges();
			}
			LoadTabData();
		}

		[RelayCommand]
		private void DeleteProjectEmployee(ProjectEmployee pe)
		{
			if (pe != null)
			{
				SoftDeleteEntity<ProjectEmployee>(pe.Id);
				pe.IsDeleted = true;
				pe.DeletedAt = DateTime.UtcNow;
				pe.DeletedReason = "Verwijderd voor administratieve redenen";
			}
			
		}

		[RelayCommand]
		private void AddMaterial()
		{
			if (SelectedMaterialId == 0 || NewQuantity <= 0) return;
			using (var context = new GreenManagerDbContext())
			{
				var materialInDb = context.Materials.Find(SelectedMaterialId);
				if (materialInDb != null)
				{
					if (NewQuantity > materialInDb.StockQuantity)
					{
						MessageBox.Show($"Niet genoeg voorraad! Er zijn slechts {materialInDb.StockQuantity} {materialInDb.Unit} beschikbaar.", "Voorraad Tekort", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}
					materialInDb.StockQuantity -= NewQuantity;
					context.Materials.Update(materialInDb);

					var newMat = new ProjectMaterial { ProjectId = EditedProject.Id, MaterialId = SelectedMaterialId, Quantity = NewQuantity };
					context.Set<ProjectMaterial>().Add(newMat);
					context.SaveChanges();
				}
			}
			NewQuantity = 0;
			LoadTabData();
		}

		[RelayCommand]
		private void DeleteProjectMaterial(ProjectMaterial pm)
		{
			if (pm != null && MessageBox.Show("Materiaal verwijderen en voorraad herstellen?", "Bevestiging", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					var entity = context.Set<ProjectMaterial>().Find(pm.Id);
					if (entity != null)
					{
						entity.IsDeleted = true;
						entity.DeletedAt = DateTime.UtcNow;
						entity.DeletedReason = "Verwijderd voor administratieve redenen";
						context.Set<ProjectMaterial>().Update(entity);

						var materialInDb = context.Materials.Find(entity.MaterialId);
						if (materialInDb != null)
						{
							materialInDb.StockQuantity += entity.Quantity;
							context.Materials.Update(materialInDb);
						}
						context.SaveChanges();
					}
				}
				LoadTabData();
			}
		}

		[RelayCommand]
		private void AddWorkLog()
		{
			if (SelectedWorkEmployeeId == 0 || NewHoursWorked <= 0) return;

			using (var context = new GreenManagerDbContext())
			{
				var employee = context.Employees.Find(SelectedWorkEmployeeId);
				decimal wageAtTime = employee != null ? employee.HourlyWage : 0;
				var newLog = new WorkLog
				{
					ProjectId = EditedProject.Id,
					EmployeeId = SelectedWorkEmployeeId,
					WorkDate = NewWorkDate,
					HoursWorked = NewHoursWorked,
					TaskDescription = NewTaskDescription,
					HourlyWageAtTime = wageAtTime
				};
				context.WorkLogs.Add(newLog);
				context.SaveChanges();
			}
			NewTaskDescription = string.Empty;
			NewHoursWorked = 0;
			LoadTabData();
		}

		[RelayCommand]
		private void DeleteWorkLog(WorkLog wl)
		{
			if (wl != null)
			{
				SoftDeleteEntity<WorkLog>(wl.Id);
				wl.IsDeleted = true;
				wl.DeletedAt = DateTime.UtcNow;
				wl.DeletedReason = "Verwijderd voor administratieve redenen";
			}
		}

		private void SoftDeleteEntity<T>(int id) where T : BaseEntity<int>
		{
			if (MessageBox.Show("Weet je zeker dat je dit record wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					var entity = context.Set<T>().Find(id);
					if (entity != null)
					{
						entity.IsDeleted = true;
						entity.DeletedAt = DateTime.UtcNow;
						entity.DeletedReason = "Verwijderd voor administratieve redenen";
						context.Set<T>().Update(entity);
						context.SaveChanges();
					}
				}
				LoadTabData();
			}
		}
		#endregion
	}
}
