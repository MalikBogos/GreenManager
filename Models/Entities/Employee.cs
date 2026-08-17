using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

		[Column(TypeName = "decimal(18,2)")]
		public decimal HourlyWage { get; set; }

		public DateTime HireDate { get; set; }

		public DateTime? DateOfBirth { get; set; }

		[StringLength(100)]
		public string? JobTitle { get; set; }

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