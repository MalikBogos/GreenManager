using GreenManager_WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for DashboardView.xaml
	/// </summary>
	public partial class DashboardView : UserControl
	{
		public DashboardView(DashboardViewModel viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
		}
	}
}
