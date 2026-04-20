using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
	public class WorkLog : BaseEntity<int>
	{
		[Required]
		public int EmployeeId { get; set; }
		public Employee Employee { get; set; } = null!;

		[Required]
		public int ProjectId { get; set; }
		public Project Project { get; set; } = null!;

		[Required]
		public DateTime WorkDate { get; set; }

		[StringLength(500)]
		public string? TaskDescription { get; set; }

		[Required]
		[Range(0.25, 24)]
		[Column(TypeName = "decimal(18,2)")]
		public decimal HoursWorked { get; set; } // 0.25 = 15 min

		// Bonus?

		[StringLength(1500)]
		public string? Notes { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal HourlyWageAtTime { get; set; }

		[NotMapped]
		public decimal LaborCost => HoursWorked * HourlyWageAtTime;

		// context.WorkLogs
		//.Where(w => w.HoursWorked* w.HourlyWageAtTime > 100)
		//.ToList();
	}
}
