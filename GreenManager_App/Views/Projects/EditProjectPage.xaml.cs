using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Projects;

public partial class EditProjectPage : ContentPage
{
	public EditProjectPage(ProjectsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}