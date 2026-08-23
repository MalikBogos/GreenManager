using GreenManager_App.ViewModels;
namespace GreenManager_App.Views.Employees;

public partial class EmployeeDetailsPage : ContentPage
{
	public EmployeeDetailsPage(EmployeesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}