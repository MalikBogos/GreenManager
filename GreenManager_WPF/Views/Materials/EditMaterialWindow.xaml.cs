using Models.Entities;
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
    /// Interaction logic for EditMaterialWindow.xaml
    /// </summary>
    public partial class EditMaterialWindow : Window
    {
		public Material EditedMaterial { get; set; }

		public EditMaterialWindow(Material selectedMaterial)
		{
			InitializeComponent();

			EditedMaterial = new Material
			{
				Id = selectedMaterial.Id,
				Name = selectedMaterial.Name,
				Description = selectedMaterial.Description,
				PurchasePrice = selectedMaterial.PurchasePrice,
				StockQuantity = selectedMaterial.StockQuantity,
				Unit = selectedMaterial.Unit,
				Notes = selectedMaterial.Notes,
				CreatedAt = selectedMaterial.CreatedAt
			};

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

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}
