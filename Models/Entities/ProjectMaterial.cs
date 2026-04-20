using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
	public class ProjectMaterial : BaseEntity<int>
	{
		[Column(TypeName = "decimal(18,2)")]
		public decimal Quantity { get; set; }

		[Required]
		public int ProjectId { get; set; }
		public Project Project { get; set; } = null!;

		[Required]
		public int MaterialId { get; set; }
		public Material Material { get; set; } = null!;
	}
}
