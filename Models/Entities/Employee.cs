using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class Employee : BaseEntity
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(20)]
		public required string EmployeeNumber { get; set; }

		public DateTime HireDate { get; set; }
		public DateTime? DateOfBirth { get; set; }

		public ICollection<Address> Addresses { get; set; } = new List<Address>();
		public Address? CurrentAddress => Addresses.FirstOrDefault(a => !a.IsDeleted);

		[StringLength(150)]
		public string? EmergencyContactName { get; set; }

		[Phone]
		[StringLength(20)]
		public string? EmergencyContactPhone { get; set; }

		public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

		public bool IsDeleted { get; set; } = false;

		[StringLength(250)]
		public string? DeletedReason { get; set; }

		public DateTime? DeletedAt { get; set; }
	}
}
