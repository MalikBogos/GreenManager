using GreenManager___WPF.Commands;
using GreenManager___WPF.Views;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GreenManager___WPF.ViewModels
{
	public class MaterialViewModel
	{
		public ObservableCollection<Material> Materials { get; set; }

		public Material SelectedMaterial { get; set; }

		public ICommand OpenAddWindowCommand { get; }

		public ICommand EditCommand { get; }

		public ICommand DeleteCommand { get; }

		public MaterialViewModel()
		{
			Materials = new ObservableCollection<Material>();
			OpenAddWindowCommand = new RelayCommand(OpenAddWindow);
			EditCommand = new RelayCommand(EditMaterial);
			DeleteCommand = new RelayCommand(DeleteMaterial);
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

		private void EditMaterial()
		{
			if (SelectedMaterial == null)
			{
				MessageBox.Show("Selecteer eerst een materiaal om te bewerken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var editWindow = new EditMaterialWindow(SelectedMaterial);

			if(editWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					context.Materials.Update(editWindow.EditedMaterial);
					context.SaveChanges();
				}
				LoadMaterials();
			}
		}

		private void DeleteMaterial()
		{
			// 1. Controleer of er wel iets is geselecteerd
			if (SelectedMaterial == null)
			{
				MessageBox.Show("Selecteer eerst een materiaal uit de lijst.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			// 2. Vraag om bevestiging (Best practice!)
			var result = MessageBox.Show($"Weet je zeker dat je '{SelectedMaterial.Name}' wilt verwijderen?", "Verwijderen bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					context.Materials.Remove(SelectedMaterial);
					context.SaveChanges();
				}

				// 3. Ververs de lijst zodat hij van het scherm verdwijnt
				LoadMaterials();
			}
		}
	}
}
