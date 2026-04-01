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
		public required int Id { get; init; }

		[Required(ErrorMessage = "Customer's FirstName is required")]
		[StringLength(100)]
		public required string FirstName { get; set; }

		
		[Required(ErrorMessage = "Customer's LastName is required")]
		[StringLength(100)]
		public required string LastName { get; set; }

		[Required(ErrorMessage = "Customer's Email is required")]
		[StringLength(200)]
		[EmailAddress]
		public required string Email { get; set; }

		[ProtectedPersonalData]
		[Phone]
		[Required(ErrorMessage = "Cannot contact customer without Customer's PhoneNumber")]
		public required string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Customer's Address is required")] // Cannot work without knowing the address
		public required string Address { get; set; }
		
		public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

		public bool IsDeleted { get; set; } = false;

		public DateTime? DeletedAt { get; set; }
	}
}
