using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
	public class EmployeeWageHistory : BaseEntity<int>
	{
		[Required]
		public int EmployeeId { get; set; }

		public Employee Employee { get; set; } = null!;

		[Required]
		[Column(TypeName="decimal(18,2)")]
		public decimal HourlyWage { get; set; }

		[Required]
		public DateTime EffectiveFrom { get; set; }

		public DateTime? EffectiveTo { get; set; }
	}
}
