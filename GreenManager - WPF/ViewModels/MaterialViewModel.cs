using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GreenManager___WPF.ViewModels
{
	public class MaterialViewModel
	{
		public ObservableCollection<Material> Materials { get; set; }

		public MaterialViewModel()
		{
			Materials = new ObservableCollection<Material>();

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
	}
}
