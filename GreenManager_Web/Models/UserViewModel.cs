using System.ComponentModel.DataAnnotations;

namespace GreenManager_Web.Models
{
	public class UserViewModel
	{
		public string Id { get; set; } = string.Empty;

		// Worden enkel gebruikt voor de weergave
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Rol is verplicht")]
		public string Role { get; set; } = string.Empty;

		public bool IsBlocked { get; set; }
	}
}