using GreenManager___WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for ProjectView.xaml
	/// </summary>
	public partial class ProjectView : UserControl
	{
		public ProjectView()
		{
			InitializeComponent();
			this.DataContext = new ProjectViewModel();
		}
	}
}
