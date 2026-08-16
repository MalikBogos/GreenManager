using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class Employee : BaseEntity<int>
	{
		[Required]
		public required string ApplicationUserId { get; set; }
		public ApplicationUser User { get; set; } = null!;

		[Required]
		[StringLength(20)]
		public required string EmployeeNumber { get; set; }

		public ICollection<EmployeeWageHistory> WageHistory { get; set; } = new List<EmployeeWageHistory>();

		// Handy computed property voor het huidige tarief
		public decimal? CurrentHourlyWage =>
			WageHistory.Where(w => w.EffectiveTo == null || w.EffectiveTo > DateTime.Now)
					   .OrderByDescending(w => w.EffectiveFrom)
					   .FirstOrDefault()?.HourlyWage;

		// .Include(e => e.WageHistory).?

		public DateTime HireDate { get; set; }

		public DateTime? DateOfBirth { get; set; }

		[StringLength(100)]
		public string? JobTitle { get; set; }

		[StringLength(100)]
		public string? Department { get; set; }

		[StringLength(150)]
		public string? EmergencyContactName { get; set; }

		[Phone]
		[StringLength(20)]
		public string? EmergencyContactPhone { get; set; }

		[StringLength(200)]
		public string? Street { get; set; }

		[StringLength(20)]
		public string? PostalCode { get; set; }

		[StringLength(100)]
		public string? City { get; set; }

		public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();

		public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();

		[StringLength(1500)]
		public string? Notes { get; set; }

	}
}