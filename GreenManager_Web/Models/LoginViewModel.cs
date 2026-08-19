using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	// Wordt gebruikt in de AccountsController voor het aanmelden van een gebruiker
	public class LoginViewModel
	{
		[Required(ErrorMessage = "E-mailadres is verplicht")]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Wachtwoord is verplicht")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Display(Name = "Onthoud mij")]
		public bool RememberMe { get; set; }
	}
}