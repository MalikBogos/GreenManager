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

		public string? CompanyName { get; set; }

		public string? VATNumber { get; set; }

		[StringLength(1500)]
		public string? Notes { get; set; }

		[Required(ErrorMessage = "Customer's Email is required. Cannot communicate without Email")]
		[StringLength(200)]
		[EmailAddress]
		public required string Email { get; set; }

		[Required(ErrorMessage = "Customer's PhoneNumber is required. Cannot contact customer without Customer's PhoneNumber")]
		[Phone]
		public required string PhoneNumber { get; set; }

		[StringLength(200)]
		public string? Street { get; set; }

		[StringLength(20)]
		public string? PostalCode { get; set; }
		
		[StringLength(100)]
		public string? City { get; set; }

		public ICollection<Project> Projects { get; set; } = new List<Project>();
	}
}
