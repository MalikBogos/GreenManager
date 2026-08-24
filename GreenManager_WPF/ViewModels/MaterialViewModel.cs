using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GreenManager_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GreenManager_WPF.ViewModels
{
	public partial class MaterialViewModel : ObservableObject
	{
		private readonly IDbContextFactory<GreenManagerDbContext> _contextFactory;

		public ObservableCollection<Material> Materials { get; set; }

		[ObservableProperty]
		private Material _selectedMaterial;

		public MaterialViewModel(IDbContextFactory<GreenManagerDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
			Materials = new ObservableCollection<Material>();
			LoadMaterials();
		}

		private void LoadMaterials()
		{
			try
			{
				using (var context = _contextFactory.CreateDbContext())
				{
					var MaterialsFromDb = context.Materials.Where(m => m.IsDeleted == false).ToList();

					Materials.Clear();

					foreach (var materials in MaterialsFromDb)
					{
						Materials.Add(materials);
					}
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij LoadMaterials(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void OpenAddWindow()
		{
			try
			{
				var addWindow = new AddMaterialWindow();

				if (addWindow.ShowDialog() == true)
				{
					using (var context = _contextFactory.CreateDbContext())
					{
						var MaterialToSave = addWindow.NewMaterial;

						context.Materials.Add(MaterialToSave);
						context.SaveChanges();
					}
					LoadMaterials();
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij OpenAddWindow(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void EditMaterial()
		{
			try
			{
				if (SelectedMaterial == null)
				{
					MessageBox.Show("Selecteer eerst een materiaal om te bewerken.", "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				var editWindow = new EditMaterialWindow(SelectedMaterial);

				if (editWindow.ShowDialog() == true)
				{
					using (var context = _contextFactory.CreateDbContext())
					{
						editWindow.EditedMaterial.UpdatedAt = DateTime.UtcNow;
						context.Materials.Update(editWindow.EditedMaterial);
						context.SaveChanges();
					}
					LoadMaterials();
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij EditMaterial(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		[RelayCommand]
		private void SoftDeleteMaterial()
		{
			try
			{
				if (SelectedMaterial == null)
				{
					MessageBox.Show($"Selecteer een materiaal om te verwijderen", "Foutmelding", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				var result = MessageBox.Show($"Ben je zeker dat je {SelectedMaterial.Name} wil verwijderen?", "Bevestiging", MessageBoxButton.YesNo, MessageBoxImage.Question);

				if (result == MessageBoxResult.Yes)
				{
					using (var context = _contextFactory.CreateDbContext())
					{
						SelectedMaterial.IsDeleted = true;
						SelectedMaterial.DeletedAt = DateTime.UtcNow;
						SelectedMaterial.DeletedReason = "Verwijderd voor archivering";
						context.Materials.Update(SelectedMaterial);
						context.SaveChanges();
					}
					LoadMaterials();
				}
			}
			catch (Exception ex)
			{
				{
					MessageBox.Show($"Er ging iets mis bij SoftDeleteMaterial(): {ex.Message}",
									"Fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
	}
}
