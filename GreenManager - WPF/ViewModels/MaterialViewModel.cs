using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
	public partial class MaterialViewModel : ObservableObject
	{
		public ObservableCollection<Material> Materials { get; set; }

		[ObservableProperty]
		private Material _selectedMaterial;

		public MaterialViewModel()
		{
			Materials = new ObservableCollection<Material>();
			LoadMaterials();
		}

		private void LoadMaterials()
		{
			using (var context = new GreenManagerDbContext())
			{
				var MaterialsFromDb = context.Materials.Where(m => m.IsDeleted == false).ToList();

				Materials.Clear();

				foreach (var materials in MaterialsFromDb)
				{
					Materials.Add(materials);
				}
			}
		}

		[RelayCommand]
		private void OpenAddWindow()
		{
			var addWindow = new AddMaterialWindow();

			if (addWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					var MaterialToSave = addWindow.NewMaterial;

					context.Materials.Add(MaterialToSave);
					context.SaveChanges();
				}
				LoadMaterials();
			}
		}

		[RelayCommand]
		private void EditMaterial()
		{
			if (SelectedMaterial == null)
			{
				MessageBox.Show("Selecteer eerst een materiaal om te bewerken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var editWindow = new EditMaterialWindow(SelectedMaterial);

			if (editWindow.ShowDialog() == true)
			{
				using (var context = new GreenManagerDbContext())
				{
					editWindow.EditedMaterial.UpdatedAt = DateTime.UtcNow;
					context.Materials.Update(editWindow.EditedMaterial);
					context.SaveChanges();
				}
				LoadMaterials();
			}
		}

		[RelayCommand]
		private void DeleteMaterial()
		{
			if (SelectedMaterial == null)
			{
				MessageBox.Show("Selecteer eerst een materiaal uit de lijst.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var result = MessageBox.Show($"Weet je zeker dat je '{SelectedMaterial.Name}' wilt verwijderen?", "Verwijderen bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				using (var context = new GreenManagerDbContext())
				{
					SelectedMaterial.IsDeleted = true;
					SelectedMaterial.DeletedAt = DateTime.UtcNow;
					SelectedMaterial.DeletedReason = "Verwijderd voor administatieve redenen";
					context.Materials.Update(SelectedMaterial);
					context.SaveChanges();
				}

				LoadMaterials();
			}
		}
	}
}
