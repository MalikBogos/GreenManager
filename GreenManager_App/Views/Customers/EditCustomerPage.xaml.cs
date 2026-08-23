using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Customers;

public partial class EditCustomerPage : ContentPage
{
	public EditCustomerPage(CustomersViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}