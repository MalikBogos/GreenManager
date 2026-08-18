using GreenManager_WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for CustomerView.xaml
	/// </summary>
	public partial class CustomerView : UserControl
	{
		public CustomerView(CustomerViewModel viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
		}
	}
}
