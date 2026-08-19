using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	// Wordt gebruikt in de AccountsController voor het aanmaken van een nieuwe gebruiker
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Voornaam is verplicht")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Achternaam is verplicht")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "E-mailadres is verplicht")]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Wachtwoord is verplicht")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		[Display(Name = "Bevestig Wachtwoord")]
		[Compare("Password", ErrorMessage = "De wachtwoorden komen niet overeen.")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}