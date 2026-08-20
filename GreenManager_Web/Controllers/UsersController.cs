using GreenManager_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Entities; 

namespace GreenManager_Web.Controllers
{
    // Controller gebruikt voor /Users/ (Gegevensbeheer, rollenbeheer etc. van gebruikers)
    // Zorgt ervoor dat enkel Admins deze acties kunnen uitvoeren
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
        // Toont alle (niet verwijderde) 'users' met gegevens en rol
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

        // GET: /Users/Edit/5
        // Opent het bewerkingsformulier en toont de gegevens van de gekozen gebruiker (email, rol, IsBlocked status)
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

        // POST: /Users/Edit/5
        // Past de IsBlocked status aan, controleert of de gebruiker een andere rol heeft gekozen dan hij momenteel heeft (en past deze aan indien wel), anders gaat hij terug naar de index
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

            // Past de gegevens van het account aan
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