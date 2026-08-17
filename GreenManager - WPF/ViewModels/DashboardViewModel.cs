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

		// Properties for the statistic cards
		[ObservableProperty]
		private int _totalCustomers;

		[ObservableProperty]
		private int _totalEmployees;

		[ObservableProperty]
		private int _totalActiveProjects;

		// Collection for the quick-overview table
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
				// 1. Calculate the statistics (Counting records)
				TotalCustomers = context.Customers.AsNoTracking().Count(c => !c.IsDeleted);
				TotalEmployees = context.Employees.AsNoTracking().Count(e => !e.IsDeleted);

				// Count projects that are actually accepted or in progress
				TotalActiveProjects = context.Projects.AsNoTracking().Count(p => !p.IsDeleted &&
																 (p.Status == ProjectStatus.Accepted ||
																  p.Status == ProjectStatus.InProgress));

				// 2. Fetch the top 5 most urgent/active projects to display on the dashboard
				var recentProjects = context.Projects
					.AsNoTracking()
					.Include(p => p.Customer) // Include customer to show the name
					.Where(p => !p.IsDeleted &&
								p.Status != ProjectStatus.Completed &&
								p.Status != ProjectStatus.Cancelled)
					.OrderBy(p => p.StartDate) // Sort by start date
					.Take(5) // Only take the top 5
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
