using GreenManager___WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager___WPF.Views
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
