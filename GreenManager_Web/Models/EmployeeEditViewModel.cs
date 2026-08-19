using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	public class EmployeeEditViewModel
	{
		public int Id { get; set; }

		public string ApplicationUserId { get; set; } = string.Empty;

		public string EmployeeNumber { get; set; } = string.Empty;

		// Inlogaccount gegevens
		[Required(ErrorMessage = "FirstName is verplicht")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "LastName is verplicht")]
		public string LastName { get; set; } = string.Empty;

		// --- Werknemers (Employee) gegevens ---
		public string JobTitle { get; set; } = string.Empty;

		[Range(0.00, 1000, ErrorMessage = "Uurloon moet groter of gelijk zijn aan 0")] // =<0 voor het vermijden van problemen bij de loonkostberekening
		public decimal HourlyWage { get; set; }

		public DateTime HireDate { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string? Street { get; set; }
		public string? PostalCode { get; set; }
		public string? City { get; set; }
		public string? Notes { get; set; }
	}
}