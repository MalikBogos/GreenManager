using GreenManager___WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GreenManager___WPF.Views
{
	/// <summary>
	/// Interaction logic for EditProjectWindow.xaml
	/// </summary>
	public partial class EditProjectWindow : Window
	{
		private ProjectViewModel _viewModel;

		public EditProjectWindow(ProjectViewModel viewModel)
		{
			InitializeComponent();

			// Koppel het meegeleverde ProjectViewModel aan de DataContext van dit scherm
			_viewModel = viewModel;
			this.DataContext = _viewModel;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_viewModel.EditedProject.Name) || _viewModel.EditedProject.CustomerId == 0)
			{
				MessageBox.Show("Projectnaam en Klant zijn verplicht.", "Validatie", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			this.DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
	}
}
