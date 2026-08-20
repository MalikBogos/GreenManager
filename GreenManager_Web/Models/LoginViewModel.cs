using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	// Wordt gebruikt in de AccountsController voor het aanmelden van een gebruiker
	public class LoginViewModel
	{
		[Required(ErrorMessage = "EmailRequired")]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "PasswordRequired")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Display(Name = "RememberMe")]
		public bool RememberMe { get; set; }
	}
}