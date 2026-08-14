using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using Models.Entities.Base;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GreenManager___WPF.Views
{
	public partial class ProjectDetailWindow : Window
	{
		public Project EditedProject { get; set; }
		public List<Customer> AvailableCustomers { get; set; }
		public List<ProjectStatus> AvailableStatuses { get; set; }

		// Collections for the DataGrids in the tabs
		public ObservableCollection<ProjectEmployee> ProjectEmployees { get; set; } = new ObservableCollection<ProjectEmployee>();
		public ObservableCollection<ProjectMaterial> ProjectMaterials { get; set; } = new ObservableCollection<ProjectMaterial>();
		public ObservableCollection<WorkLog> WorkLogs { get; set; } = new ObservableCollection<WorkLog>();

		// Lists for the Dropdown menus in the tabs
		public List<Employee> AvailableEmployees { get; set; } = new List<Employee>();
		public List<Material> AvailableMaterials { get; set; } = new List<Material>();

		// Inputs for Tab 2: Planning
		public int SelectedEmployeeId { get; set; }
		public DateTime NewPlannedDate { get; set; } = DateTime.Today;
		public decimal NewEstimatedHours { get; set; }

		// Inputs for Tab 3: Materials
		public int SelectedMaterialId { get; set; }
		public decimal NewQuantity { get; set; }

		// Inputs for Tab 4: WorkLogs
		public int SelectedWorkEmployeeId { get; set; }
		public DateTime NewWorkDate { get; set; } = DateTime.Today;
		public decimal NewHoursWorked { get; set; }
		public string NewTaskDescription { get; set; } = string.Empty;

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
			LoadTabData();
		}

		private void LoadTabData()
		{
			using (var context = new GreenManagerDbContext())
			{
				// Load Dropdowns
				AvailableEmployees = context.Employees.Include(e => e.User).Where(e => !e.IsDeleted).OrderBy(e => e.User.LastName).ToList();
				AvailableMaterials = context.Materials.Where(m => !m.IsDeleted).OrderBy(m => m.Name).ToList();

				// Load Project Employees
				var pEmployees = context.Set<ProjectEmployee>()
					.Include(pe => pe.Employee)
					.Where(pe => pe.ProjectId == EditedProject.Id && !pe.IsDeleted).ToList();
				ProjectEmployees.Clear();
				foreach (var pe in pEmployees) ProjectEmployees.Add(pe);

				// Load Project Materials
				var pMaterials = context.Set<ProjectMaterial>()
					.Include(pm => pm.Material)
					.Where(pm => pm.ProjectId == EditedProject.Id && !pm.IsDeleted).ToList();
				ProjectMaterials.Clear();
				foreach (var pm in pMaterials) ProjectMaterials.Add(pm);

				// Load WorkLogs
				var wLogs = context.WorkLogs
					.Include(w => w.Employee)
					.Where(w => w.ProjectId == EditedProject.Id && !w.IsDeleted).ToList();
				WorkLogs.Clear();
				foreach (var w in wLogs) WorkLogs.Add(w);
			}
		}

		// --- TAB 2: PLANNING LOGIC ---
		private void BtnAddEmployee_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedEmployeeId == 0 || NewEstimatedHours <= 0) return;

			using (var context = new GreenManagerDbContext())
			{
				var newPlan = new ProjectEmployee
				{
					ProjectId = EditedProject.Id,
					EmployeeId = SelectedEmployeeId,
					PlannedDate = NewPlannedDate,
					EstimatedHours = NewEstimatedHours
				};
				context.Set<ProjectEmployee>().Add(newPlan);
				context.SaveChanges();
			}
			LoadTabData();
		}

		private void BtnDeleteProjectEmployee_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as Button)?.DataContext is ProjectEmployee pe) SoftDeleteEntity<ProjectEmployee>(pe.Id);
		}

		// --- TAB 3: MATERIALS LOGIC ---
		private void BtnAddMaterial_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedMaterialId == 0 || NewQuantity <= 0) return;

			using (var context = new GreenManagerDbContext())
			{
				var newMat = new ProjectMaterial
				{
					ProjectId = EditedProject.Id,
					MaterialId = SelectedMaterialId,
					Quantity = NewQuantity
				};
				context.Set<ProjectMaterial>().Add(newMat);
				context.SaveChanges();
			}
			LoadTabData();
		}

		private void BtnDeleteProjectMaterial_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as Button)?.DataContext is ProjectMaterial pm) SoftDeleteEntity<ProjectMaterial>(pm.Id);
		}

		// --- TAB 4: WORKLOG LOGIC ---
		private void BtnAddWorkLog_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedWorkEmployeeId == 0 || NewHoursWorked <= 0) return;

			using (var context = new GreenManagerDbContext())
			{
				// SMART LOGIC: Find the active wage for this specific date!
				var wageHistory = context.Set<EmployeeWageHistory>()
					.Where(w => w.EmployeeId == SelectedWorkEmployeeId && !w.IsDeleted && w.EffectiveFrom <= NewWorkDate && (w.EffectiveTo == null || w.EffectiveTo >= NewWorkDate))
					.OrderByDescending(w => w.EffectiveFrom)
					.FirstOrDefault();

				decimal wageAtTime = wageHistory != null ? wageHistory.HourlyWage : 0;

				if (wageAtTime == 0)
				{
					MessageBox.Show("Let op: Er is geen geldig loon gevonden voor deze datum. Loonkosten worden op €0,00 gezet.", "Geen loonhistoriek", MessageBoxButton.OK, MessageBoxImage.Information);
				}

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

			// Reset input and reload
			NewTaskDescription = string.Empty;
			NewHoursWorked = 0;
			LoadTabData();
		}

		private void BtnDeleteWorkLog_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as Button)?.DataContext is WorkLog wl) SoftDeleteEntity<WorkLog>(wl.Id);
		}

		// --- HELPER METODS & MAIN BUTTONS ---
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
						context.Set<T>().Update(entity);
						context.SaveChanges();
					}
				}
				LoadTabData();
			}
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