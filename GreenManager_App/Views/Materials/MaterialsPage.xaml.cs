using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Materials;

public partial class MaterialsPage : ContentPage
{
	private readonly MaterialsViewModel _viewModel;

	public MaterialsPage(MaterialsViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		_viewModel.LoadMaterialsCommand.Execute(null);
	}
}