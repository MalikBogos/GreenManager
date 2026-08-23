using GreenManager_App.ViewModels;
namespace GreenManager_App.Views.Projects;

public partial class ProjectDetailsPage : ContentPage
{
	public ProjectDetailsPage(ProjectsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

	}
}