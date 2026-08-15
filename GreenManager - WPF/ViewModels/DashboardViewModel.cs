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
		// Properties for the statistic cards
		[ObservableProperty]
		private int _totalCustomers;

		[ObservableProperty]
		private int _totalEmployees;

		[ObservableProperty]
		private int _totalActiveProjects;

		// Collection for the quick-overview table
		public ObservableCollection<Project> ActiveProjects { get; set; }

		public DashboardViewModel()
		{
			ActiveProjects = new ObservableCollection<Project>();
			LoadDashboardData();
		}

		private void LoadDashboardData()
		{
			using (var context = new GreenManagerDbContext())
			{
				// 1. Calculate the statistics (Counting records)
				TotalCustomers = context.Customers.Where(c => !c.IsBlocked).Count(c => !c.IsDeleted);
				TotalEmployees = context.Employees.Count(e => !e.IsDeleted);

				// Count projects that are actually accepted or in progress
				TotalActiveProjects = context.Projects.Count(p => !p.IsDeleted &&
																 (p.Status == ProjectStatus.Accepted ||
																  p.Status == ProjectStatus.InProgress));

				// 2. Fetch the top 5 most urgent/active projects to display on the dashboard
				var recentProjects = context.Projects
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
