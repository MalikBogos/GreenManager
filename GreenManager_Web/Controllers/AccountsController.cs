using GreenManager_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace GreenManager_Web.Controllers
{
	// Controller die wordt gebruikt voor het aanmaken van accounts en login, /Accounts/Login en /Accounts/Register
	public class AccountsController : Controller
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;

		// Dependency Injection
		public AccountsController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		// GET: /Accounts/Login
		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		// POST: /Accounts/Login
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);

				// Controleert of de gebruiker ongeldig is zodat hij niet kan inloggegn
				if (user == null || user.IsBlocked || user.IsDeleted)
				{
					ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens of account geblokkeerd");
					return View(model);
				}

				// Resultaat (of de signin succesvol was ( of niet)
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				// Indien het resultaat succesvol is
				if (result.Succeeded)
				{
					// Stuur de gebruiker terug naar waar hij heen wilde, of naar de Homepagina
					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
						return LocalRedirect(returnUrl);
					else
						return RedirectToAction("Index", "Home");
				}
				else // Toon fout dat er iets mis is gelopen
				{
					ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens of account geblokkeerd.");
				}
			}

			return View(model);
		}

		// GET: /Accounts/Register
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		// POST: /Accounts/Register
		/// <summary> 
		/// (Reference voor documentatiestijl)
		/// Verwerkt de registratie van een nieuwe gebruiker: valideert het model,
		/// maakt een ApplicationUser aan op basis van de ingevulde RegisterViewModel-gegevens,
		/// geeft de standaardrol "Guest" en logt de gebruiker automatisch in
		/// </summary>

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				// Maak een nieuwe gebruiker aan met deze velden van ApplicationUser (Gebaseerd op mijn eigen identity user class met extra eigenschappen)
				var user = new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					CreatedAt = System.DateTime.UtcNow
				};

				// Resultaat (of de aanmaking succesvol was of niet)
				var result = await _userManager.CreateAsync(user, model.Password);

				// Indien aanmaking succesvol was
				if (result.Succeeded)
				{
					// Geef de nieuwe gebruiker automatisch de rol 'Guest'
					await _userManager.AddToRoleAsync(user, "Guest");

					// Log de gebruiker in
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("Index", "Home");
				}

				// Toon foutbescrijving
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return View(model);
		}

		// POST: /Accounts/Logout
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		// GET: /Accounts/AccessDenied
		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}