using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Models.Entities
{
	public class ApplicationUser : IdentityUser
	{
		// Necessary
		//[ProtectedPersonalData]
		[Required(ErrorMessage = "FirstName is required")]
		[StringLength(50)] //Programming standard
		public required string FirstName { get; set; }

		// Necessary
		//[ProtectedPersonalData]
		[Required(ErrorMessage = "LastName is required")]
		[StringLength(100)] //Programming standard
		public required string LastName { get; set; }

		// In case a user gets fired, for example (set instead of init in case user needs to be unblocked)
		public bool IsBlocked { get; set; } = false;

		[StringLength(250, ErrorMessage = "Block reason should be 250 characters at most")]
		public string? BlockReason { get; set; }

		public DateTime? BlockedAt { get; set; }

		// Necessary information for account info
		[Required(ErrorMessage = "Application User creation date is required")]
		public required DateTime CreatedAt { get; init; } = DateTime.UtcNow;

		// Soft-delete
		//[Required(ErrorMessage = "IsDeleted of Application User is required")]
		public bool IsDeleted { get; set; } = false;

		// Necessary information for account info
		public DateTime? DeletedAt { get; set; }
	}
}