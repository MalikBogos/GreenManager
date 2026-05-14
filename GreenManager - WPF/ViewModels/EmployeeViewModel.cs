using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GreenManager___WPF.ViewModels
{
	public class EmployeeViewModel
	{
		public ObservableCollection<Employee> Employees { get; set; }

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
	}
}
