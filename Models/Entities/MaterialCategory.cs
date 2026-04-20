using Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{
	public class MaterialCategory : BaseEntity<int>
	{
		[Required]
		[StringLength(200)]
		public required string Name { get; set; }
		public ICollection<Material> Materials { get; set; } = new List<Material>();

		[StringLength(1500)]
		public string? Description { get; set; }

	}
}
