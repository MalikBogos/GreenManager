using CommunityToolkit.Mvvm.ComponentModel;
using Models.Data;
using Models.Entities;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GreenManager___WPF.ViewModels
{
	public partial class DashboardViewModel : ObservableObject
	{
		private readonly IDbContextFactory<GreenManagerDbContext> _contextFactory;

		[ObservableProperty]
		private int _totalCustomers;

		[ObservableProperty]
		private int _totalEmployees;

		[ObservableProperty]
		private int _totalActiveProjects;

		public ObservableCollection<Project> ActiveProjects { get; set; }

		public DashboardViewModel(IDbContextFactory<GreenManagerDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
			ActiveProjects = new ObservableCollection<Project>();
			LoadDashboardData();
		}

		private void LoadDashboardData()
		{
			using (var context = _contextFactory.CreateDbContext())
			{
				TotalCustomers = context.Customers.AsNoTracking().Count(c => !c.IsDeleted);
				TotalEmployees = context.Employees.AsNoTracking().Count(e => !e.IsDeleted);

				TotalActiveProjects = context.Projects.AsNoTracking().Count(p => !p.IsDeleted &&
																 (p.Status == ProjectStatus.Accepted ||
																  p.Status == ProjectStatus.InProgress));

				var recentProjects = context.Projects
					.AsNoTracking()
					.Include(p => p.Customer)
					.Where(p => !p.IsDeleted &&
								p.Status != ProjectStatus.Completed &&
								p.Status != ProjectStatus.Cancelled)
					.OrderBy(p => p.StartDate)
					.Take(5)
					.ToList();

				ActiveProjects.Clear();
				foreach (var project in recentProjects)
				{
					ActiveProjects.Add(project);
				}
			}
		}
	}
}
