using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	public class EmployeeCreateViewModel
	{
		// Inlogaccount gegevens, aangemaakt met de newUser = new ApplicationUser in POST: EMPLOYEES/CREATE
		[Required(ErrorMessage = "FirstName is verplicht")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "LastName is verplicht")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "EmailAddress is verplicht")]
		[EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
		public string Email { get; set; } = string.Empty;

		// Employee gegevens 
		public string EmployeeNumber { get; set; } = string.Empty; // Aangemaakt in GET: Employees/Create tijdens het openen van het Create formulier en daarna hieraan gekoppeld

		public string JobTitle { get; set; } = string.Empty;

		[Range(0.00, 1000, ErrorMessage = "Uurloon moet groter of gelijk zijn aan 0")] // =<0 voor het vermijden van problemen bij de loonkostberekening
		public decimal HourlyWage { get; set; }

		public DateTime HireDate { get; set; } = DateTime.Today;

		public DateTime? DateOfBirth { get; set; }

		public string? Street { get; set; }
		public string? PostalCode { get; set; }
		public string? City { get; set; }
		public string? Notes { get; set; }
	}
}