using SQLite;

namespace GreenManager_App.Models.Local
{
	/// <summary>
	/// Vlakke, lokale representatie van een werknemer. FirstName/LastName staan hier plat op,
	/// terwijl ze op de server verspreid zitten over Employee + het gekoppelde ApplicationUser.
	/// </summary>
	public class LocalEmployee
	{
		[PrimaryKey, AutoIncrement]
		public int LocalId { get; set; }

		public int ServerId { get; set; }

		public string EmployeeNumber { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? JobTitle { get; set; }
		public decimal HourlyWage { get; set; }
		public DateTime HireDate { get; set; }
		public string? Notes { get; set; }
	}
}