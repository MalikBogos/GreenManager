using GreenManager___WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for EmployeeView.xaml
	/// </summary>
	public partial class EmployeeView : UserControl
	{
		public EmployeeView()
		{
			InitializeComponent();
			this.DataContext = new EmployeeViewModel();
		}
	}
}
