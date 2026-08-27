using SQLite;

namespace GreenManager_App.Models.Local
{
	/// <summary>
	/// Vlakke, lokale representatie van een materiaal — heeft geen enkele navigatie-eigenschap
	/// nodig, aangezien Material zelf ook geen relaties bevat die de weergave beïnvloeden.
	/// </summary>
	public class LocalMaterial
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public int ServerId { get; set; }

		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string Unit { get; set; } = string.Empty;
		public decimal PurchasePrice { get; set; }
		public decimal StockQuantity { get; set; }
		public string? Notes { get; set; }
	}
}