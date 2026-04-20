using Microsoft.EntityFrameworkCore;
using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class Address : BaseEntity<int>
	{
		public int? CustomerId { get; set; }
		public Customer? Customer { get; set; } // Attention

		public int? EmployeeId { get; set; }
		public Employee? Employee { get; set; } // Attention: maybe possible to make an exclusive implementation to lock out either one of the options in case one is assigned?

		// Voorbeeldstraat 31A
		[Required(ErrorMessage = "Street is required")]
		[StringLength(200)]
		public required string AddressLine1 { get; set; }

		// Bus 10 / Appartement 2 
		[StringLength(100)]
		public string? AddressLine2 { get; set; }

		[Required(ErrorMessage = "PostalCode is required")]
		[StringLength(10)]
		public required string PostalCode { get; set; }

		[Required(ErrorMessage = "City is required")]
		[StringLength(100)]
		public required string City { get; set; }

		[StringLength(100)]
		public string? Province { get; set; }

		[Required(ErrorMessage = "Country is required")]
		[StringLength(100)]
		public string Country { get; set; } = "Belgium";

		[StringLength(50)]
		public string AddressType { get; set; } = "Home";
	}
}
