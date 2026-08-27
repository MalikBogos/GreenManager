using SQLite;

namespace GreenManager_App.Models.Local
{
	/// <summary>
	/// Vlakke, lokale representatie van een project. CustomerLastName staat er plat op,
	/// naar het voorbeeld van ProjectDto — dit vermijdt een lokale join met LocalCustomer.
	/// </summary>
	public class LocalProject
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public int ServerId { get; set; }

		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public int CustomerId { get; set; }
		public string CustomerLastName { get; set; } = string.Empty;
		public string? ProjectAddress { get; set; }
		public decimal? Budget { get; set; }
		public string? Notes { get; set; }
	}
}