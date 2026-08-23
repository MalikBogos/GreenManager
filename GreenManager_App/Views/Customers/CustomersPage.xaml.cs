using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Customers;

public partial class CustomersPage : ContentPage
{
	private readonly CustomersViewModel _viewModel;

	public CustomersPage(CustomersViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	/// <summary>
	/// Deze ingebouwde MAUI methode wordt automatisch uitgevoerd voordat de pagina in beeld komt
	/// </summary>
	protected override void OnAppearing()
	{
		base.OnAppearing();

		// Voer het LoadCustomersCommand uit dat in het ViewModel staat
		_viewModel.LoadCustomersCommand.Execute(null);
	}
}