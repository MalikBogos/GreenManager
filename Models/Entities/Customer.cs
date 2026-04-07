using Microsoft.AspNetCore.Identity;
using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class Customer : BaseEntity<int>
	{

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

		//[Required(ErrorMessage = "Customer's Address is required. Cannot carry out work without knowing Customer's Address")]
		//[StringLength(250)]
		public ICollection<Address> Addresses { get; set; } = new List<Address>(); // Removed 'required' to enable lazy loading?

		public Address? CurrentAddress => Addresses.FirstOrDefault(a => !a.IsDeleted);

		// For example in case a customer is problematic
		public bool IsBlocked { get; set; } = false;

		[StringLength(250, ErrorMessage = "Block reason should be 250 characters at most")]
		public string? BlockedReason { get; set; }

		public DateTime? BlockedAt { get; set; }
	}
}
