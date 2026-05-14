using Models.Data;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GreenManager___WPF.ViewModels
{
	public class ProjectViewModel
	{
		public ObservableCollection<Project> Projects { get; set; }

		public ProjectViewModel()
		{
			Projects = new ObservableCollection<Project>();

			LoadProjects();
		}

		private void LoadProjects()
		{
			using (var context = new GreenManagerDbContext())
			{
				var ProjectsFromDb = context.Projects.ToList();

				foreach(var project in ProjectsFromDb)
				{
					Projects.Add(project);
				}
			}
		}
	}
}
