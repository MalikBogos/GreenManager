using GreenManager___WPF.Commands;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace GreenManager___WPF.ViewModels
{
	public class MaterialViewModel
	{
		public ObservableCollection<Material> Materials { get; set; }

		public string NewName { get; set; } = "";
		public string NewUnit { get; set; } = "";
		public decimal NewPrice { get; set; }
		public decimal NewStock { get; set; }

		public ICommand SaveCommand { get; }

		public MaterialViewModel()
		{
			Materials = new ObservableCollection<Material>();
			SaveCommand = new RelayCommand(SaveMaterial);
			LoadMaterials();
		}

		private void LoadMaterials()
		{
			using (var context = new GreenManagerDbContext())
			{
				var MaterialsFromDb = context.Materials.ToList();

				foreach(var materials in MaterialsFromDb)
				{
					Materials.Add(materials);
				}
			}
		}

		private void SaveMaterial()
		{
			if (string.IsNullOrWhiteSpace(NewName)) return;

			using (var context = new GreenManagerDbContext())
			{
				var m = new Material
				{
					Name = NewName,
					Unit = NewUnit,
					PurchasePrice = NewPrice,
					StockQuantity = NewPrice,
					CreatedAt = DateTime.UtcNow
				};

				context.Materials.Add(m);
				context.SaveChanges();
			}

			LoadMaterials();
		}
	}
}
