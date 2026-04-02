using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	[Owned]
	public class Address
	{
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
	}
}
