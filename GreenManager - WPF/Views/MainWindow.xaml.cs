using GreenManager___WPF.ViewModels;
using Models.Entities;
using System.Windows;

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(ApplicationUser loggedInUser)
		{
			InitializeComponent();
			this.DataContext = new MainViewModel(loggedInUser);
		}

		private void BtnDashboard_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new DashboardView();
		}

		private void BtnClients_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new CustomerView();
		}

		private void BtnProjects_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new ProjectView();
		}

		private void BtnMaterials_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new MaterialView();
		}


		private void BtnEmployees_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new EmployeeView();
		}

		private void BtnSettings_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = new SettingsView();
		}
	}
}