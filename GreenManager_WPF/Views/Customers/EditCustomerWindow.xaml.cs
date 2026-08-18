using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for EditCustomerWindow.xaml
	/// </summary>
	public partial class EditCustomerWindow : Window
	{
		public Customer EditedCustomer { get; set; }

		public EditCustomerWindow(Customer selectedCustomer)
		{
			InitializeComponent();

			EditedCustomer = new Customer
			{
				Id = selectedCustomer.Id,
				FirstName = selectedCustomer.FirstName,
				LastName = selectedCustomer.LastName,
				CompanyName = selectedCustomer.CompanyName,
				VATNumber = selectedCustomer.VATNumber,
				Email = selectedCustomer.Email,
				PhoneNumber = selectedCustomer.PhoneNumber,
				Street = selectedCustomer.Street,
				PostalCode = selectedCustomer.PostalCode,
				City = selectedCustomer.City,
				Notes = selectedCustomer.Notes,
				CreatedAt = selectedCustomer.CreatedAt
			};

			this.DataContext = this;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}
