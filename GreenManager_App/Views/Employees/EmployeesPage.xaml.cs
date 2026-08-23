using GreenManager_App.ViewModels;
namespace GreenManager_App.Views.Employees;

public partial class EmployeesPage : ContentPage
{
	private readonly EmployeesViewModel _viewModel;
	public EmployeesPage(EmployeesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		_viewModel.LoadEmployeesCommand.Execute(null);
	}
}