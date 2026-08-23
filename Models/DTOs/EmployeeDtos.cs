using System;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
	// Platte weergave van een werknemer (GET)
	public class EmployeeDto
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string EmployeeNumber { get; set; } = string.Empty;
		public decimal HourlyWage { get; set; }
		public DateTime HireDate { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? JobTitle { get; set; }
		public string? Street { get; set; }
		public string? PostalCode { get; set; }
		public string? City { get; set; }
		public string? Notes { get; set; }
	}

	// Platte invoer voor een nieuwe of te bewerken werknemer (POST/PUT)
	public class EmployeeRequestDto
	{
		[Required(ErrorMessage = "Voornaam is verplicht.")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Achternaam is verplicht.")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "E-mailadres is verplicht.")]
		[EmailAddress(ErrorMessage = "Ongeldig e-mailadres.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Werknemersnummer is verplicht.")]
		public string EmployeeNumber { get; set; } = string.Empty;

		public decimal HourlyWage { get; set; }
		public DateTime HireDate { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? JobTitle { get; set; }
		public string? Street { get; set; }
		public string? PostalCode { get; set; }
		public string? City { get; set; }
		public string? Notes { get; set; }
	}
}