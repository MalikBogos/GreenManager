using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Projects;

public partial class AddProjectPage : ContentPage
{
	public AddProjectPage(ProjectsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}