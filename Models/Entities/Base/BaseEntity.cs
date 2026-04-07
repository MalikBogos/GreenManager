using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities.Base
{
	public abstract class BaseEntity<TId> where TId : notnull
	{
		[Key]
		public TId Id { get; init; } = default!;
		public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

		public DateTime? UpdatedAt { get; set; }

		public bool IsDeleted { get; set; } = false;

		[StringLength(250)]
		public string? DeletedReason { get; set; }

		public DateTime? DeletedAt { get; set; }
	}
}
