using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager___WPF.Views;
using Microsoft.EntityFrameworkCore;
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
	public partial class CustomerViewModel : ObservableObject
	{
		private readonly IDbContextFactory<GreenManagerDbContext> _contextFactory;

		public ObservableCollection<Customer> Customers { get; set; }

		[ObservableProperty]
		private Customer _selectedCustomer;

		[ObservableProperty]
		private string _searchQuery = string.Empty;

		partial void OnSearchQueryChanged(string value)
		{
			LoadCustomers();
		}

		public CustomerViewModel(IDbContextFactory<GreenManagerDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
			Customers = new ObservableCollection<Customer>();
			LoadCustomers();
		}

		private void LoadCustomers()
        {
			using (var context = _contextFactory.CreateDbContext())
			{
				var query = context.Customers.Where(c => c.IsDeleted == false).AsQueryable();

				if (!string.IsNullOrWhiteSpace(SearchQuery))
				{
					query = query.Where(c => c.FirstName.Contains(SearchQuery) ||
												c.LastName.Contains(SearchQuery) ||
												c.Email.Contains(SearchQuery) ||
												(c.CompanyName != null && c.CompanyName.Contains(SearchQuery)));
				}

				// Voer de zoekopdracht uit
				var customersFromDb = query.ToList();

				Customers.Clear();
				foreach (var customer in customersFromDb)
				{
					Customers.Add(customer);
				}
			}
            
        }

		[RelayCommand]
		private void OpenAddWindow()
		{
			var addWindow = new AddCustomerWindow();

			if (addWindow.ShowDialog() == true)
			{
				using (var context = _contextFactory.CreateDbContext())
				{
					var CustomerToSave = addWindow.NewCustomer;

					context.Customers.Add(CustomerToSave);
					context.SaveChanges();
				}
				LoadCustomers();
			}
		}

		[RelayCommand]
		private void EditCustomer()
		{
			if (SelectedCustomer == null)
			{
				MessageBox.Show("Selecteer eerst een klant om te bewerken.");
				return;
			}

			var editWindow = new EditCustomerWindow(SelectedCustomer);

			using (var context = _contextFactory.CreateDbContext()) {
				if (editWindow.ShowDialog() == true)
				{
					editWindow.EditedCustomer.UpdatedAt = DateTime.UtcNow;
					context.Customers.Update(editWindow.EditedCustomer);
					context.SaveChanges();
				}
				LoadCustomers();
			}
		}

		[RelayCommand]
		private void SoftDeleteCustomer()
		{
			if (SelectedCustomer == null)
			{
				MessageBox.Show($"Selecteer een materiaal om te verwijderen", "Foutmelding", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Ben je zeker dat je {SelectedCustomer.FirstName} wil verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);
			using (var context = _contextFactory.CreateDbContext()) {
				if (result == MessageBoxResult.Yes)
				{
					SelectedCustomer.IsDeleted = true;
					SelectedCustomer.DeletedAt = DateTime.UtcNow;
					SelectedCustomer.DeletedReason = "Verwijderd voor archivering";
					context.Customers.Update(SelectedCustomer);
					context.SaveChanges();
				}

				LoadCustomers();
			}
		}
	}
}
