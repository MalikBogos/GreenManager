using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Models.Entities.Base;
using Models.Enums;

namespace Models.Entities
{
	public class Quote : BaseEntity<int>
	{
		[Required]
		public int ProjectId { get; set; }
		public Project Project { get; set; } = null!;

		[Required]
		public DateTime QuoteDate { get; set; }

		[Required]
		public QuoteStatus Status { get; set; }

		public ICollection<QuoteItem> Items { get; set; } = new List<QuoteItem>();

		[StringLength(1500)]
		public string? Notes { get; set; }
	}
}
