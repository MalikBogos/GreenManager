using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace GreenManager___WPF.ViewModels
{
	public partial class EmployeeViewModel : ObservableObject
	{
		public ObservableCollection<Employee> Employees { get; set; }

		[ObservableProperty]
		private Employee _selectedEmployee;
		
		public EmployeeViewModel()
		{
			Employees = new ObservableCollection<Employee>();
			LoadEmployees();
		}

		private void LoadEmployees()
		{
			using (var context = new GreenManagerDbContext())
			{
				var EmployeesFromDb = context.Employees.ToList();

				foreach (var employee in EmployeesFromDb)
				{
					Employees.Add(employee);
				}
			}
		}

		[RelayCommand]
		private void OpenAddWindow()
		{
			// TODO: Dit activeren zodra we de AddEmployeeWindow hebben gemaakt!
			// var addWindow = new AddEmployeeWindow();
			// if (addWindow.ShowDialog() == true)
			// {
			//     using (var context = new GreenManagerDbContext())
			//     {
			//         context.Employees.Add(addWindow.NewEmployee);
			//         context.SaveChanges();
			//     }
			//     LoadEmployees();
			// }
			MessageBox.Show("Het venster voor een nieuwe werknemer bouwen we in de volgende stap!");
		}

		[RelayCommand]
		private void EditEmployee()
		{
			if (SelectedEmployee == null)
			{
				MessageBox.Show("Selecteer eerst een werknemer om te bewerken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			// TODO: Dit activeren zodra we de EditEmployeeWindow hebben gemaakt!
			// var editWindow = new EditEmployeeWindow(SelectedEmployee);
			// if (editWindow.ShowDialog() == true)
			// {
			//     using (var context = new GreenManagerDbContext())
			//     {
			//         context.Employees.Update(editWindow.EditedEmployee);
			//         context.SaveChanges();
			//     }
			//     LoadEmployees();
			// }
			MessageBox.Show($"We gaan binnenkort de gegevens van {SelectedEmployee.JobTitle} bewerken!");
		}

		[RelayCommand]
		private void DeleteEmployee()
		{
			if (SelectedEmployee == null)
			{
				MessageBox.Show("Selecteer eerst een werknemer om te verwijderen.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var result = MessageBox.Show($"Weet je zeker dat je werknemer '{SelectedEmployee.EmployeeNumber}' wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					SelectedEmployee.IsDeleted = true; // Soft delete
					context.Employees.Update(SelectedEmployee);
					context.SaveChanges();
				}
				LoadEmployees();
			}
		}
	}
}
