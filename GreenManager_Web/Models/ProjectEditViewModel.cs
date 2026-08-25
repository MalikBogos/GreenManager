using System.ComponentModel.DataAnnotations;
using Models.Entities;
using Models.Enums;

namespace GreenManager_Web.Models
{
	public class ProjectEditViewModel
	{
		public int Id { get; set; }

		public DateTime CreatedAt { get; set; }

		[Required(ErrorMessage = "NameRequired")]
		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		[Required(ErrorMessage = "StartDateRequired")]
		public DateTime StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public ProjectStatus Status { get; set; }

		[Required(ErrorMessage = "CustomerRequired")]
		public int CustomerId { get; set; }

		public string? ProjectAddress { get; set; }

		public decimal? Budget { get; set; }

		public string? Notes { get; set; }

		// Gebruikt om de tabellen op de edit pagina met data te laden
		public IEnumerable<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
		public IEnumerable<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();
		public IEnumerable<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
	}
}