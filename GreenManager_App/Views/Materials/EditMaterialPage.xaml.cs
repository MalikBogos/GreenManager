using GreenManager_App.ViewModels;
namespace GreenManager_App.Views.Materials;


public partial class EditMaterialPage : ContentPage
{
	public EditMaterialPage(MaterialsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}