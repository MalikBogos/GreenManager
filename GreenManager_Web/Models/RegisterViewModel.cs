using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "FirstNameRequired")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "LastNameRequired")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "EmailRequired")]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "PasswordRequired")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		[Display(Name = "ConfirmPassword")]
		[Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}