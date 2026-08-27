using SQLite;

namespace GreenManager_App.Models.Local
{
	/// <summary>
	/// Vlakke, lokale representatie van een klant, opgeslagen in de SQLite-databank op het toestel.
	/// Bewust géén navigatie-eigenschappen (zoals bij Project.Customer) — dat zou joins vereisen
	/// die lokaal onnodig traag zijn; alle nodige weergavegegevens staan hier plat op de rij zelf.
	/// </summary>
	public class LocalCustomer
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public int ServerId { get; set; }

		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? CompanyName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
	}
}