using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Models.Data;
using Models.Entities;

namespace GreenManager___WPF.ViewModels
{
	public class CustomerViewModel
	{
		public ObservableCollection<Customer> Customers { get; set; }

		public CustomerViewModel()
		{
			Customers = new ObservableCollection<Customer>();

			LoadCustomers();
		}

		private void LoadCustomers()
		{
			using (var context = new GreenManagerDbContext())
			{
				var customersFromDb = context.Customers.ToList();

				foreach (var customer in customersFromDb)
				{
					Customers.Add(customer);
				}
			}
		}

	}
}
