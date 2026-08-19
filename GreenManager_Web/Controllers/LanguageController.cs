using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace GreenManager_Web.Controllers
{
	public class LanguageController : Controller
	{
		//private readonly GreenManagerDbContext _context;

		//public LanguageController(GreenManagerDbContext context)
		//{
		//	_context = context;
		//}

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
