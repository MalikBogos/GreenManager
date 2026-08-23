using GreenManager_App.ViewModels;
namespace GreenManager_App.Views.Employees;

public partial class EditEmployeePage : ContentPage
{
	public EditEmployeePage(EmployeesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}