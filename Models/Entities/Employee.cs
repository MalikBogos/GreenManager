using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class Employee : BaseEntity<int>
	{
		[Required]
		public required string ApplicationUserId { get; set; }
		public ApplicationUser User { get; set; } = null!;

		[Required]
		[StringLength(20)]
		public required string EmployeeNumber { get; set; }

		public DateTime HireDate { get; set; }
		public DateTime? DateOfBirth { get; set; }

		[StringLength(100)]
		public string? JobTitle { get; set; }

		[StringLength(100)]
		public string? Department { get; set; }

		[StringLength(150)]
		public string? EmergencyContactName { get; set; }

		[Phone]
		[StringLength(20)]
		public string? EmergencyContactPhone { get; set; }

		public ICollection<Address> Addresses { get; set; } = new List<Address>();
		public Address? CurrentAddress => Addresses.FirstOrDefault(a => !a.IsDeleted);
	}
}
