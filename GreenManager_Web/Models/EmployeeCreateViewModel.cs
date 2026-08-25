using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	public class EmployeeCreateViewModel
	{
		// Inlogaccount gegevens, aangemaakt met de newUser = new ApplicationUser in POST: EMPLOYEES/CREATE
		[Required(ErrorMessage = "FirstNameRequired")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "LastNameRequired")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "EmailRequired")]
		[EmailAddress(ErrorMessage = "EmailInvalid")]
		public string Email { get; set; } = string.Empty;

		// Employee gegevens 
		public string EmployeeNumber { get; set; } = string.Empty; // Aangemaakt in GET: Employees/Create tijdens het openen van het Create formulier en daarna hieraan gekoppeld

		[Required(ErrorMessage = "JobTitleRequired")]
		public string JobTitle { get; set; } = string.Empty;

		[Range(0.00, 1000, ErrorMessage = "HourlyWageInvalid")] // =<0 voor het vermijden van problemen bij de loonkostberekening
		public decimal HourlyWage { get; set; }

		public DateTime HireDate { get; set; } = DateTime.Today;

		public DateTime? DateOfBirth { get; set; }

		public string? Street { get; set; }
		public string? PostalCode { get; set; }
		public string? City { get; set; }
		public string? Notes { get; set; }
	}
}