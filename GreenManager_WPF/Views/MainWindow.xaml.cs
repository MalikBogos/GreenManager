using GreenManager_WPF.ViewModels;
using Models.Entities;
using System.Windows;
using GreenManager_WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;

		public MainWindow(MainViewModel viewModel)
		{
			InitializeComponent();

			_viewModel = viewModel;

			this.DataContext = viewModel;

			MainContent.Content = App.AppHost.Services.GetRequiredService<DashboardView>();
		}

		public void InitializeUser(ApplicationUser loggedinUser, string roleName) 
		{
			_viewModel.SetupUser(loggedinUser, roleName);
		}

		private void BtnDashboard_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = App.AppHost.Services.GetRequiredService<DashboardView>();
		}

		private void BtnClients_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = App.AppHost.Services.GetRequiredService<CustomerView>();
		}

		private void BtnProjects_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = App.AppHost.Services.GetRequiredService<ProjectView>();
		}

		private void BtnMaterials_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = App.AppHost.Services.GetRequiredService<MaterialView>();
		}


		private void BtnEmployees_Click(object sender, RoutedEventArgs e)
		{
			MainContent.Content = App.AppHost.Services.GetRequiredService<EmployeeView>();
		}
	}
}