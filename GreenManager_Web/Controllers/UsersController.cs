using GreenManager_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Entities; 

namespace GreenManager_Web.Controllers
{
	/// <summary>
	/// Beheert gebruikers op de /Users pagina om accounts te blokkeren of van rol te wijzigen. Alleen gebruikers met de rol 'Admin' mogen deze acties uitvoeren, anders wordt de gebruiker doorverwezen naar de 'toegang beperkt' pagina (/Accounts/AccessDenied).
	/// </summary>
	[Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

		// GET: /Users
		/// <summary>
		/// Toont een overzicht van alle, niet-verwijderde gebruikers met hun huidige rol en blokkeringsstatus.
		/// </summary>
		/// <returns>/Views/Users/Index</returns>
		public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Where(u => !u.IsDeleted).ToListAsync();
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                // Laadt de 'users' via de foreach-loop
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "Guest",
                    IsBlocked = user.IsBlocked
                });
            }

            return View(model);
        }

		// GET: /Users/Edit/{Id}
		/// <summary>
		/// Toont het bewerkingsformulier van een specifieke gebruiker om de rol of blokkeringsstatus aan te passen.
		/// </summary>
		/// <param name="id">Verwijst naar het Identity Id van de op te halen gebruiker.</param>
		/// <returns>/Views/Users/Edit/{Id} of een 404NotFound-pagina indien de gebruiker niet bestaat.</returns>
		public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

            var model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "Guest",
                IsBlocked = user.IsBlocked
            };

            return View(model);
        }

		// POST: /Users/Edit/{Id}
		/// <summary>
		/// Verwerkt /Users/Edit/{Id} om de rol of de blokkeringsstatus van een gebruiker aan te passen in de database.
		/// </summary>
		/// <param name="model">>Verwijst naar de bewerkte gegevens die werden doorgeeven aan het UserViewModel.</param>
		/// <returns>Redirect naar /Views/Users/Index bij succes, of herlaadt de pagina indien er een fout optreedt.</returns>
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Past de blokkeringsstatus van het account aan
            user.IsBlocked = model.IsBlocked;

            await _userManager.UpdateAsync(user);

            // Controleer of de rol gewijzigd is en pas deze aan
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.FirstOrDefault() != model.Role && !string.IsNullOrEmpty(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}