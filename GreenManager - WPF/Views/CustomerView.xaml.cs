using GreenManager___WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for CustomerView.xaml
	/// </summary>
	public partial class CustomerView : UserControl
	{
		public CustomerView()
		{
			InitializeComponent();
			this.DataContext = new CustomerViewModel();
		}
	}
}
