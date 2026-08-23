using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Customers;

public partial class CustomerDetailsPage : ContentPage
{
	public CustomerDetailsPage(CustomersViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}