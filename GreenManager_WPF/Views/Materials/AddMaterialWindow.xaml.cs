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
using GreenManager_WPF.ViewModels;
using Models.Entities;

namespace GreenManager_WPF.Views
{
	/// <summary>
	/// Interaction logic for AddMaterialWindow.xaml
	/// </summary>
	public partial class AddMaterialWindow : Window
	{
		public Material NewMaterial { get; set; }

		public AddMaterialWindow()
		{
			InitializeComponent();

			NewMaterial = new Material { Name = "Voorbeeld", Description = "Dit is een description", PurchasePrice = 3, StockQuantity = 20, Unit = "Dit is een unit", Notes = "Dit zijn notities", };

			this.DataContext = this;
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

	}
}
