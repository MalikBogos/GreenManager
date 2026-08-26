using GreenManager_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Enums;
using System.Diagnostics;

namespace GreenManager_Web.Controllers
{
	/// <summary>
	/// Beheert de Home/Dashboard-pagina waar de gebruiker belandt bij het inloggen alsook de Error-pagina.
	/// </summary>
	public class HomeController : Controller
	{
		private readonly GreenManagerDbContext _context;

		public HomeController(GreenManagerDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Toont een Dashboard met statistieken aan gebruikers met de rol 'Admin' of 'Employee', anders de Toegang Beperkt-pagina.
		/// </summary>
		/// <returns>/Views/Home/Index met statistieken of een Toegang Beperkt-pagina</returns>
		public async Task<IActionResult> Index()
		{
			// Doe de berekeningen enkel als de gebruiker een medewerker of admin is
			if (User.IsInRole("Admin") || User.IsInRole("Employee"))
			{
				// Tel het aantal actieve klanten, werknemers & actieve projecten (met status Accepted of InProgresss) en zet ze in ViewBags zodat ze in de index.html geladen kunnen worden
				ViewBag.TotalCustomers = await _context.Customers.CountAsync(c => !c.IsDeleted);
				ViewBag.TotalEmployees = await _context.Employees.CountAsync(e => !e.IsDeleted);
				ViewBag.TotalActiveProjects = await _context.Projects.CountAsync(p => !p.IsDeleted && (p.Status == ProjectStatus.Accepted || p.Status == ProjectStatus.InProgress));

				// Haal de 5 dichtsbijzijnde startdate actieve projecten op
				ViewBag.ActiveProjects = await _context.Projects
					.Include(p => p.Customer)
					.Where(p => !p.IsDeleted && (p.Status == ProjectStatus.Accepted || p.Status == ProjectStatus.InProgress))
					.OrderBy(p => p.StartDate)
					.Take(5)
					.ToListAsync();
			}
			return View();
		}

		// Toont de algemene foutpagina als er een exception optreedt.
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
