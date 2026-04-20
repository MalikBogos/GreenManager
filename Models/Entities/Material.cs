using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
	public class Material : BaseEntity<int>
	{
		[StringLength(500)]
		public required string Name { get; set; }

		[StringLength(1500)]
		public string? Description { get; set; }

		[StringLength(20)]
		public string? Unit { get; set; } // bijv. "m²", "stuk", "liter"

		[Column(TypeName = "decimal(18,2)")]
		public decimal PurchasePrice { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal StockQuantity { get; set; }

		public int? CategoryId { get; set; }
		public MaterialCategory? Category { get; set; }

		public ICollection<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();

		[StringLength(1500)]
		public string? Notes { get; set; }

	}
}
