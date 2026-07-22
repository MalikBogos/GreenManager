using GreenManager___WPF.Commands;
using GreenManager___WPF.Views;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GreenManager___WPF.ViewModels
{
	public class CustomerViewModel
	{
		public ObservableCollection<Customer> Customers { get; set; }
		
		public Customer SelectedCustomer { get; set; }

		public ICommand OpenAddWindowCommand { get; }

		public ICommand EditCommand { get; }

		public ICommand DeleteCommand { get; }

		public CustomerViewModel()
		{
			Customers = new ObservableCollection<Customer>();
			OpenAddWindowCommand = new RelayCommand(OpenAddWindow);
			EditCommand = new RelayCommand(EditCustomer);
			DeleteCommand = new RelayCommand(DeleteCustomer);
			LoadCustomers();
		}

		private void OpenAddWindow()
		{
			var addWindow = new AddCustomerWindow();

			if (addWindow.ShowDialog() == true)
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

		private void LoadCustomers()
		{
			using (var context = new GreenManagerDbContext())
			{
				var customersFromDb = context.Customers.Where(c => c.IsDeleted == false).ToList();

				Customers.Clear();

				foreach (var customer in customersFromDb)
				{
					Customers.Add(customer);
				}
			}
		}

		private void EditCustomer()
		{
			if (SelectedCustomer == null)
			{
				MessageBox.Show("Selecteer eerst een klant om te bewerken.");
				return;
			}

			var editWindow = new EditCustomerWindow(SelectedCustomer);

			if (editWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					context.Customers.Update(editWindow.EditedCustomer);
					context.SaveChanges();
				}
				LoadCustomers();
			}
		}

		private void DeleteCustomer()
		{
			if (SelectedCustomer == null)
			{
				MessageBox.Show("Selecteer eerst een klant om te verwijderen.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var result = MessageBox.Show($"Weet je zeker dat je {SelectedCustomer.FirstName} {SelectedCustomer.LastName} wilt verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					SelectedCustomer.IsDeleted = true;
					context.Customers.Update(SelectedCustomer);
					context.SaveChanges();
				}

				LoadCustomers();
			}
		}



	}
}
