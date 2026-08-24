using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_WPF.Views;
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

namespace GreenManager_WPF.ViewModels
{
	public partial class ProjectViewModel : ObservableObject
	{
		#region --- HOOFDSCHERM  ---
		private readonly IDbContextFactory<GreenManagerDbContext> _contextFactory;
		
		public ObservableCollection<Project> Projects { get; set; }

		[ObservableProperty]
		private Project _selectedProject;

		public ProjectViewModel(IDbContextFactory<GreenManagerDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
			Projects = new ObservableCollection<Project>();
			ProjectEmployees = new ObservableCollection<ProjectEmployee>();
			ProjectMaterials = new ObservableCollection<ProjectMaterial>();
			WorkLogs = new ObservableCollection<WorkLog>();

			LoadProjects();
		}

		private void LoadProjects()
		{
			using (var context = _contextFactory.CreateDbContext())
			{
				var ProjectsFromDb = context.Projects.Where(p => !p.IsDeleted).ToList();

				Projects.Clear();
				foreach (var project in ProjectsFromDb)
				{
					Projects.Add(project);
				}
			}
		}

		[RelayCommand] 
		private void OpenAddProjectWindow()
		{
			using (var context = _contextFactory.CreateDbContext())
			{
				var activeCustomers = context.Customers.Where(c => c.IsDeleted == false).OrderBy(c => c.LastName).ToList();

				if (activeCustomers.Count == 0)
				{
					MessageBox.Show("Voeg eerst een klant toe voordat je een project of offerte kunt aanmaken.", "Geen klanten gevonden", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				var addWindow = new AddProjectWindow(activeCustomers);

				if (addWindow.ShowDialog() == true)
				{
					context.Projects.Add(addWindow.NewProject);
					context.SaveChanges();

					LoadProjects();
				}
			}
		}

		#endregion

		#region --- BEWERKSCHERM  ---

		[ObservableProperty] private Project _editedProject;
		[ObservableProperty] private List<Customer> _availableCustomers;
		[ObservableProperty] private List<ProjectStatus> _availableStatuses;
		[ObservableProperty] private List<Employee> _availableEmployees;
		[ObservableProperty] private List<Material> _availableMaterials;

		public ObservableCollection<ProjectEmployee> ProjectEmployees { get; set; }
		public ObservableCollection<ProjectMaterial> ProjectMaterials { get; set; }
		public ObservableCollection<WorkLog> WorkLogs { get; set; }

		// Tab 2: Planning
		[ObservableProperty] private int _selectedEmployeeId;
		[ObservableProperty] private DateTime _newPlannedDate = DateTime.Today;
		[ObservableProperty] private decimal _newEstimatedHours;

		// Tab 3: Materials
		[ObservableProperty] private int _selectedMaterialId;
		[ObservableProperty] private decimal _newQuantity;

		// Tab 4: WorkLogs
		[ObservableProperty] private int _selectedWorkEmployeeId;
		[ObservableProperty] private DateTime _newWorkDate = DateTime.Today;
		[ObservableProperty] private decimal _newHoursWorked;
		[ObservableProperty] private string _newTaskDescription = string.Empty;

		[RelayCommand]
		private void EditProject()
		{
			try
			{
				if (SelectedProject == null)
				{
					MessageBox.Show("Selecteer eerst een project om te bewerken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				using (var context = _contextFactory.CreateDbContext())
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

				var editWindow = new EditProjectWindow(this);
				if (editWindow.ShowDialog() == true)
				{
					using (var context = _contextFactory.CreateDbContext())
					{
						EditedProject.UpdatedAt = DateTime.UtcNow;
						context.Projects.Update(EditedProject);
						context.SaveChanges();
					}
					LoadProjects();
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij EditProject(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void LoadTabData()
		{
			try
			{
				using (var context = _contextFactory.CreateDbContext())
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
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij LoadTabData(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void SoftDeleteProject()
		{
			try
			{
				if (SelectedProject == null)
				{
					MessageBox.Show("Selecteer eerst een project om te verwijderen.", "Foutmelding", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				var result = MessageBox.Show($"Ben je zeker dat je het project '{SelectedProject.Name}' wil verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

				if (result == MessageBoxResult.Yes)
				{
					using (var context = _contextFactory.CreateDbContext())
					{
						SelectedProject.IsDeleted = true;
						SelectedProject.DeletedAt = DateTime.UtcNow;
						SelectedProject.DeletedReason = "Verwijderd via projectenbeheer";

						context.Projects.Update(SelectedProject);
						context.SaveChanges();
					}
					LoadProjects();
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij SoftDeleteProject(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		#endregion

		#region --- TABBLAD Commands ---

		[RelayCommand]
		private void AddEmployee()
		{
			try
			{
				if (SelectedEmployeeId == 0 || NewEstimatedHours <= 0) return;
				using (var context = _contextFactory.CreateDbContext())
				{
					var newPlan = new ProjectEmployee { ProjectId = EditedProject.Id, EmployeeId = SelectedEmployeeId, PlannedDate = NewPlannedDate, EstimatedHours = NewEstimatedHours };
					context.Set<ProjectEmployee>().Add(newPlan);
					context.SaveChanges();
				}
				LoadTabData();
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij AddEmployee(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void DeleteProjectEmployee(ProjectEmployee pe)
		{
			try
			{
				if (pe != null)
				{
					SoftDeleteEntity<ProjectEmployee>(pe.Id);
					pe.IsDeleted = true;
					pe.DeletedAt = DateTime.UtcNow;
					pe.DeletedReason = "Verwijderd voor administratieve redenen";
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij DeleteProjectEmployee(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}

		}

		[RelayCommand]
		private void AddMaterial()
		{
			try
			{
				if (SelectedMaterialId == 0 || NewQuantity <= 0) return;
				using (var context = _contextFactory.CreateDbContext())
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
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij AddMaterial(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void DeleteProjectMaterial(ProjectMaterial pm)
		{
			try
			{
				if (pm != null && MessageBox.Show("Materiaal verwijderen en voorraad herstellen?", "Bevestiging", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					using (var context = _contextFactory.CreateDbContext())
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
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij DeleteProjectMaterial(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void AddWorkLog()
		{
			try
			{
				if (SelectedWorkEmployeeId == 0 || NewHoursWorked <= 0) return;

				using (var context = _contextFactory.CreateDbContext())
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
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij AddWorkLog(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void DeleteWorkLog(WorkLog wl)
		{
			try
			{
				if (wl != null)
				{
					SoftDeleteEntity<WorkLog>(wl.Id);
					wl.IsDeleted = true;
					wl.DeletedAt = DateTime.UtcNow;
					wl.DeletedReason = "Verwijderd voor administratieve redenen";
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij DeleteWorkLog(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void SoftDeleteEntity<T>(int id) where T : BaseEntity<int>
		{
			try
			{
				if (MessageBox.Show("Weet je zeker dat je dit record wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					using (var context = _contextFactory.CreateDbContext())
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
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij SoftDeleteEntity(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
		#endregion
	}
}
