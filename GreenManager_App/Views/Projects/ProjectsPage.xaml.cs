using GreenManager_App.ViewModels;

namespace GreenManager_App.Views.Projects;

public partial class ProjectsPage : ContentPage
{
	private readonly ProjectsViewModel _viewModel;

	public ProjectsPage(ProjectsViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		_viewModel.LoadProjectsCommand.Execute(null);
	}
}