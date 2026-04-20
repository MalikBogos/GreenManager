using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
	public class QuoteItem : BaseEntity<int>
	{
		[Required]
		public int QuoteId { get; set; }
		public Quote Quote { get; set; } = null!;

		[StringLength(1500)]
		public required string Description { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Quantity { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal UnitPrice { get; set; }

		[NotMapped]
		public decimal TotalPrice => Quantity * UnitPrice;

		[StringLength(1500)]
		public string? Notes { get; set; }
	}
}
