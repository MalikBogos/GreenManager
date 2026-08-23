using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Customers;

public partial class AddCustomerPage : ContentPage
{
	public AddCustomerPage(CustomersViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}