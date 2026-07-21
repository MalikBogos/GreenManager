using GreenManager___WPF.Commands;
using GreenManager___WPF.Views;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace GreenManager___WPF.ViewModels
{
	public class CustomerViewModel
	{
		public ObservableCollection<Customer> Customers { get; set; }

		public ICommand OpenAddWindowCommand { get; }

		public ICommand EditCommand { get; }

		public ICommand DeleteCommand { get; }

		public CustomerViewModel()
		{
			Customers = new ObservableCollection<Customer>();
			OpenAddWindowCommand = new RelayCommand(OpenAddWindow);
			LoadCustomers();
		}

		private void LoadCustomers()
		{
			using (var context = new GreenManagerDbContext())
			{
				var customersFromDb = context.Customers.ToList();

				Customers.Clear();

				foreach (var customer in customersFromDb)
				{
					Customers.Add(customer);
				}
			}
		}

		private void OpenAddWindow()
		{
			var addWindow = new AddCustomerWindow();

			if(addWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					var CustomerToSave = addWindow.NewCustomer;

					context.Customers.Add(CustomerToSave);
					context.SaveChanges();
				}
				LoadCustomers();
			}
		}

	}
}
