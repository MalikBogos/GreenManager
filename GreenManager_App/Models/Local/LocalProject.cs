using SQLite;

namespace GreenManager_App.Models.Local
{
	/// <summary>
	/// Platte, lokale representatie van een project (Project) die wordt opgeslagen in de SQLite-databank op het toestel. Navigatie-eigenschappen ontbreken opzettelijk om Joins te vermijden.
	/// </summary>
	public class LocalProject
	{
		/// <summary>
		/// PrimaryKey van de lokale SQLite database en is enkel relevant op een lokaal toestel.
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		/// <summary>
		/// PrimaryKey van de universele SQLServer database voor het object. Het is de bedoeling dat de lokale rij overeenstemt met de universele rij tijdens de synchronisatie en blijft 0 zolang de offline aangemaakte klant niet naar de API verstuurd wordt.
		/// </summary>
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