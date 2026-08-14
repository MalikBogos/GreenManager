using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for WageHistoryWindow.xaml
	/// </summary>
	public partial class WageHistoryWindow : Window
	{
		public ObservableCollection<EmployeeWageHistory> Wages { get; set; }

		// Properties bound to the input fields for a new wage
		public decimal NewWageAmount { get; set; }
		public DateTime NewEffectiveFrom { get; set; } = DateTime.Today;

		private readonly int _employeeId;

		public WageHistoryWindow(Employee employee)
		{
			InitializeComponent();
			_employeeId = employee.Id;
			Wages = new ObservableCollection<EmployeeWageHistory>();

			this.DataContext = this;
			LoadWages();
		}

		private void LoadWages()
		{
			using (var context = new GreenManagerDbContext())
			{
				// Note: We use Set<EmployeeWageHistory>() to safely access the table
				var wagesFromDb = context.Set<EmployeeWageHistory>()
					.Where(w => w.EmployeeId == _employeeId && w.IsDeleted == false)
					.OrderByDescending(w => w.EffectiveFrom)
					.ToList();

				Wages.Clear();
				foreach (var wage in wagesFromDb)
				{
					Wages.Add(wage);
				}
			}
		}

		private void BtnAddWage_Click(object sender, RoutedEventArgs e)
		{
			if (NewWageAmount <= 0)
			{
				MessageBox.Show("Vul een geldig, positief uurloon in.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			using (var context = new GreenManagerDbContext())
			{
				// SMART LOGIC: Find the currently active wage to set its end date!
				var previousWage = context.Set<EmployeeWageHistory>()
					.Where(w => w.EmployeeId == _employeeId && w.EffectiveTo == null && w.IsDeleted == false)
					.OrderByDescending(w => w.EffectiveFrom)
					.FirstOrDefault();

				if (previousWage != null)
				{
					// End the previous wage exactly when the new one starts
					previousWage.EffectiveTo = NewEffectiveFrom;

					// NEW: We are modifying this record, so we update the timestamp!
					previousWage.UpdatedAt = DateTime.UtcNow;

					context.Set<EmployeeWageHistory>().Update(previousWage);
				}

				// Create the brand new wage record
				var newWage = new EmployeeWageHistory
				{
					EmployeeId = _employeeId,
					HourlyWage = NewWageAmount,
					EffectiveFrom = NewEffectiveFrom
				};

				context.Set<EmployeeWageHistory>().Add(newWage);
				context.SaveChanges();
			}

			// Tell the parent window to reload the employee list when closed
			this.DialogResult = true;

			// Reset input and reload the table to show the new data
			NewWageAmount = 0;
			LoadWages();
		}

		private void BtnDeleteWage_Click(object sender, RoutedEventArgs e)
		{
			// Figure out which row the user clicked on
			var button = sender as System.Windows.Controls.Button;
			var wageToDelete = button?.DataContext as EmployeeWageHistory;

			if (wageToDelete == null) return;

			var result = MessageBox.Show($"Weet je zeker dat je dit loon ({wageToDelete.HourlyWage:C}) wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					var entity = context.Set<EmployeeWageHistory>().Find(wageToDelete.Id);

					if (entity != null)
					{
						// 1. Apply Soft Delete with audit fields
						entity.IsDeleted = true;
						entity.DeletedAt = DateTime.UtcNow;
						// Assuming you added DeletedReason to BaseEntity, if not, you can remove this line!
						entity.DeletedReason = "Foutieve looninvoer verwijderd";

						context.Set<EmployeeWageHistory>().Update(entity);

						// 2. Smart Logic: If we delete the newest wage, we must "re-open" the previous one!
						var previousWage = context.Set<EmployeeWageHistory>()
							.Where(w => w.EmployeeId == _employeeId && w.IsDeleted == false && w.Id != entity.Id)
							.OrderByDescending(w => w.EffectiveFrom)
							.FirstOrDefault();

						if (previousWage != null && previousWage.EffectiveTo != null)
						{
							previousWage.EffectiveTo = null; // Re-open by clearing the end date
							previousWage.UpdatedAt = DateTime.UtcNow; // Record the modification time
							context.Set<EmployeeWageHistory>().Update(previousWage);
						}

						context.SaveChanges();
					}
				}

				// Refresh the table to hide the deleted item
				LoadWages();

				// Ensure the main window knows a change was made so it can update the CurrentHourlyWage column
				this.DialogResult = true;
			}
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
