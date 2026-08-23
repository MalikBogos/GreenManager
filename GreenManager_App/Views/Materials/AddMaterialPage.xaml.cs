using GreenManager_App.ViewModels;
namespace GreenManager_App.Views.Materials;

public partial class AddMaterialPage : ContentPage
{
	public AddMaterialPage(MaterialsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}