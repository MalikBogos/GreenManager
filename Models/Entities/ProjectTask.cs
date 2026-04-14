using Models.Entities.Base;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
	public class ProjectTask : BaseEntity<int>
	{
		[Required]
		public int ProjectId { get; set; }
		public Project Project { get; set; } = null!;

		[StringLength(250)]
		public required string Title { get; set; }

		[StringLength(1500)]
		public string? Description { get; set; }

		[Column(TypeName = "decimal(18,2)")]

		public decimal? EstimatedHours { get; set; }

		public required ProjectTaskStatus Status { get; set; }

		[StringLength(1500)]
		public string? Notes { get; set; }
	}
}
