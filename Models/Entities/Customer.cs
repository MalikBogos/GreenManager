using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class Customer
	{
		[Key]
		public int Id { get; init; }

		[Required(ErrorMessage = "Customer's FirstName is required. Cannot properly set up project without First Name")]
		[StringLength(100)]
		public required string FirstName { get; set; }

		[Required(ErrorMessage = "Customer's LastName is required. Cannot properly set up project without Last Name")]
		[StringLength(100)]
		public required string LastName { get; set; }

		[Required(ErrorMessage = "Customer's Email is required. Cannot communicate without Email")]
		[StringLength(200)]
		[EmailAddress]
		public required string Email { get; set; }

		[Required(ErrorMessage = "Customer's PhoneNumber is required. Cannot contact customer without Customer's PhoneNumber")]
		[Phone]
		public required string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Customer's Address is required. Cannot carry out work without knowing Customer's Address")]
		//[StringLength(250)]
		public required Address Address { get; set; }

		// For example in case a customer is problematic
		public bool IsBlocked { get; set; } = false;

		[StringLength(250, ErrorMessage = "Block reason should be 250 characters at most")]
		public string? BlockedReason { get; set; }

		public DateTime? BlockedAt { get; set; }

		public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

		public bool IsDeleted { get; set; } = false;

		public string? DeletedReason { get; set; }

		public DateTime? DeletedAt { get; set; }
	}
}
