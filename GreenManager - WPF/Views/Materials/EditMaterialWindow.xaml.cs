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

			// We maken een KOPIE van de data. Zo verpesten we de tabel niet als iemand op annuleren drukt.
			EditedMaterial = new Material
			{
				Id = selectedMaterial.Id, // ID moet behouden blijven voor de Update!
				Name = selectedMaterial.Name,
				Description = selectedMaterial.Description,
				PurchasePrice = selectedMaterial.PurchasePrice,
				StockQuantity = selectedMaterial.StockQuantity,
				Unit = selectedMaterial.Unit,
				Notes = selectedMaterial.Notes,
				CreatedAt = selectedMaterial.CreatedAt // Behoud de originele datum
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
