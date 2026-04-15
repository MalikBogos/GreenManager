using Models.Entities.Base;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class Project : BaseEntity<int>
	{
		[Required(ErrorMessage = "A Name is required")]
		[StringLength(200)]
		public required string Name { get; set; }

		[StringLength(1500)]
		public string? Description { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public ProjectStatus Status { get; set; }

		[Required]
		public int CustomerId { get; set; }
		public Customer Customer { get; set; } = null!;

		public string? ProjectAddress { get; set; }

		public decimal? Budget { get; set; }

		public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
		public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
		public ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();
		public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
		public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

		[StringLength(1500)]
		public string? Notes { get; set; }
	}
}
