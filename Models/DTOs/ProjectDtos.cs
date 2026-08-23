using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
	
	public class ProjectDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public string? ProjectAddress { get; set; }
		public decimal? Budget { get; set; }
		public string? Notes { get; set; }

		public int CustomerId { get; set; }
		public string CustomerFirstName { get; set; } = string.Empty;
		public string CustomerLastName { get; set; } = string.Empty;

	}

	public class ProjectRequestDto
	{
		[Required(ErrorMessage = "Naam is verplicht.")]
		[StringLength(200)]
		public string Name { get; set; } = string.Empty;

		[StringLength(1500)]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Startdatum is verplicht.")]
		public DateTime StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		[Required(ErrorMessage = "Status is verplicht.")]
		public string Status { get; set; } = string.Empty;

		[Required(ErrorMessage = "Er moet een klant gekoppeld worden.")]
		public int CustomerId { get; set; }

		public string? ProjectAddress { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "Budget kan niet negatief zijn.")]
		public decimal? Budget { get; set; }

		[StringLength(1500)]
		public string? Notes { get; set; }
	}
}