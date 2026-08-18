using GreenManager_WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	public partial class ProjectView : UserControl
	{
		public ProjectView(ProjectViewModel viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
		}
	}
}
