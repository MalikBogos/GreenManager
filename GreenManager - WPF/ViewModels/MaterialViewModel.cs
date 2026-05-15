using GreenManager___WPF.Commands;
using GreenManager___WPF.Views;
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
		public ICommand OpenAddWindowCommand { get; }

		public MaterialViewModel()
		{
			Materials = new ObservableCollection<Material>();
			OpenAddWindowCommand = new RelayCommand(OpenAddWindow);
			LoadMaterials();
		}

		private void LoadMaterials()
		{
			using (var context = new GreenManagerDbContext())
			{
				var MaterialsFromDb = context.Materials.ToList();

				Materials.Clear();

				foreach(var materials in MaterialsFromDb)
				{
					Materials.Add(materials);
				}
			}
		}

		private void OpenAddWindow()
		{
			var addWindow = new AddMaterialWindow();

			if(addWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					var materialToSave = addWindow.NewMaterial;

					context.Materials.Add(materialToSave);
					context.SaveChanges();
				}
				LoadMaterials();
			}
		}
	}
}
