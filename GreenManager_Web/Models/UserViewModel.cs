using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	public class UserViewModel
	{
		public string Id { get; set; } = string.Empty;

		[Required(ErrorMessage = "FirstName is verplicht")]
		[Display(Name = "FirstName")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "LastName is verplicht")]
		[Display(Name = "LastName")]
		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string Role { get; set; } = string.Empty;

		public bool IsBlocked { get; set; }
	}
}