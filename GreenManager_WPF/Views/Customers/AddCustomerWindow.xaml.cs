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
	/// Interaction logic for AddCustomerWindow.xaml
	/// </summary>
	public partial class AddCustomerWindow : Window
	{
		public Customer NewCustomer { get; set; }

		public AddCustomerWindow()
		{
			InitializeComponent();
			NewCustomer = new Customer { FirstName = "Tester", LastName="Code", CompanyName="Code testers", VATNumber="31414", Notes="Het is een voorbeeld", Email = "dsaiiw@gmail.com", PhoneNumber = "3442234233", Street = "Voorbeeldstraat", PostalCode = "1500", City = "Halle"};
			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}
