using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace GreenManager_Web.Models
{
	public class ProjectCreateViewModel
	{
		[Required(ErrorMessage = "NameRequired")]
		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		[Required(ErrorMessage = "StartDateRequired")]
		public DateTime StartDate { get; set; } = DateTime.Today;

		public DateTime? EndDate { get; set; }

		public ProjectStatus Status { get; set; } = ProjectStatus.Quotation;

		[Required(ErrorMessage = "CustomerRequired")]
		public int CustomerId { get; set; }

		public string? ProjectAddress { get; set; }

		public decimal? Budget { get; set; }

		public string? Notes { get; set; }
	}
}