using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
	public class ProjectEmployee : BaseEntity<int>
	{
		[Required]
		public int ProjectId { get; set; }
		public Project Project { get; set; } = null!;

		[Required]
		public int EmployeeId { get; set; }
		public Employee Employee { get; set; } = null!;

		public DateTime PlannedDate { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal EstimatedHours { get; set; }

		[StringLength(1500)]
		public string? Notes { get; set; }
	}
}
