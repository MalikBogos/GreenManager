using GreenManager_WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for MaterialView.xaml
	/// </summary>
	public partial class MaterialView : UserControl
	{
		public MaterialView(MaterialViewModel viewModel)
		{
			InitializeComponent();
			this.DataContext = viewModel;
		}
	}
}
