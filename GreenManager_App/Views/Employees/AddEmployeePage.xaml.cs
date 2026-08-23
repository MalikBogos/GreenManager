using GreenManager_App.ViewModels;
namespace GreenManager_App.Views.Employees;

public partial class AddEmployeePage : ContentPage
{
	public AddEmployeePage(EmployeesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}