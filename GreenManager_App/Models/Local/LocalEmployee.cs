using SQLite;

namespace GreenManager_App.Models.Local
{
	/// <summary>
	/// Platte, lokale representatie van een werknemer (Employee) die wordt opgeslagen in de SQLite-databank op het toestel. Navigatie-eigenschappen ontbreken opzettelijk om Joins te vermijden.
	/// </summary>
	public class LocalEmployee
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

		public string EmployeeNumber { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? JobTitle { get; set; }
		public decimal HourlyWage { get; set; }
		public DateTime HireDate { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Street { get; set; }
		public string? PostalCode { get; set; }
		public string? City { get; set; }
		public string? Notes { get; set; }
	}
}