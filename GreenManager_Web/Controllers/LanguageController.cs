using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace GreenManager_Web.Controllers
{
	/// <summary>
	/// Beheert de weergavetaal van de applicatie voor de huidige gebruiker door middel van cookies.
	/// </summary>
	public class LanguageController : Controller
	{
		// POST: /Language/SetLanguage
		/// <summary>
		/// Bewaart de gekozen cultuur/taal in een cookie voor de duur van 1 jaar en laadt de pagina opnieuw.
		/// </summary>
		/// <param name="culture">De culture-code (nl/fr/en) van de gekozen taal.</param>
		/// <param name="returnUrl">De pagina waarnaar de gebruiker teruggestuurd moet worden nadat de taal is gewijzigd.</param>
		[HttpPost]
		public IActionResult SetLanguage(string culture, string returnUrl)
		{
			// Sla de gekozen taal op in de cookie
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			// Stuur de gebruiker terug naar de pagina waar hij vandaan kwam
			return LocalRedirect(returnUrl);
		}
	}
}
