using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace GreenManager_Web.Models
{
	public class ProjectCreateViewModel
	{
		[Required(ErrorMessage = "Naam is verplicht")]
		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		[Required(ErrorMessage = "StartDate is verplicht")]
		public DateTime StartDate { get; set; } = DateTime.Today;

		public DateTime? EndDate { get; set; }

		public ProjectStatus Status { get; set; } = ProjectStatus.Quotation;

		[Required(ErrorMessage = "CustomerId is verplicht")]
		public int CustomerId { get; set; }

		public string? ProjectAddress { get; set; }

		public decimal? Budget { get; set; }

		public string? Notes { get; set; }
	}
}