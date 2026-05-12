using GreenManager___WPF.ViewModels;
using System.Windows.Controls;

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for SettingsView.xaml
	/// </summary>
	public partial class SettingsView : UserControl
	{
		public SettingsView()
		{
			InitializeComponent();
			this.DataContext = new SettingsViewModel();
		}
	}
}
