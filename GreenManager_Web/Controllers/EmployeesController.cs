using GreenManager_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

/// <summary>
/// Beheert de pagina's voor de CRUD-acties op /Employees (Werknemers). Alleen toegankelijk voor gebruikers met de rol 'Admin', anders wordt de gebruiker doorverwezen naar de 'toegang beperkt' pagina (/Accounts/AccessDenied).
/// </summary>
[Authorize(Policy = "AdminOnly")]
public class EmployeesController : Controller
{
	private readonly GreenManagerDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;

	public EmployeesController(GreenManagerDbContext context, UserManager<ApplicationUser> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	// GET: EMPLOYEES
	/// <summary>
	/// Toont een overzicht van alle, niet soft-deleted werknemers, inclusief de gekoppelde inloggegevens (.Include(e => e.User)).
	/// </summary>
	/// <returns>/Views/Employees/Index met een overzicht van actieve werknemers.</returns>
	public async Task<IActionResult> Index()
	{
		var activeEmployees = await _context.Employees.Include(e => e.User).Where(e => !e.IsDeleted).ToListAsync();
		return View(activeEmployees);
	}

	// GET: EMPLOYEES/Details/{Id}
	/// <summary>
	/// Toont een overzicht met alle details van een specifieke, niet soft-deleted Employee, inclusief inloggegevens.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Employees database-tabel van de specifieke werknemer die werd opgehaald.</param>
	/// <returns>/Views/Employees/Details/{Id} of een 404NotFound-pagina indien de werknemer niet bestaat.</returns>
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(m => m.Id == id);
		if (employee == null)
		{
			return NotFound();
		}

		return View(employee);
	}

	// GET: EMPLOYEES/Create
	/// <summary>
	/// Toont een formulier om een nieuwe Employee aan te maken. Genereert automatisch een oplopend EmployeeNumber (bv. EMP004).
	/// </summary>
	/// <returns>/Views/Employees/Create met een Employee aanmaak-formulier.</returns>
	public IActionResult Create()
	{
		// Code voor de automatische generatie van de EmployeeNumber bij het openen van het aanmaak formulier
		string newEmployeeNumber = "EMP001";
		var lastEmployee = _context.Employees.OrderByDescending(e => e.Id).FirstOrDefault();

		if (lastEmployee != null && lastEmployee.EmployeeNumber != null && lastEmployee.EmployeeNumber.StartsWith("EMP"))
		{
			string numberPart = lastEmployee.EmployeeNumber.Substring(3);
			if (int.TryParse(numberPart, out int lastNumber))
			{
				newEmployeeNumber = $"EMP{(lastNumber + 1):D3}";
			}
		}

		// Gebruik het ViewModel
		var vm = new EmployeeCreateViewModel
		{
			EmployeeNumber = newEmployeeNumber
		};
		return View(vm);
	}

	// POST: EMPLOYEES/Create
	/// <summary>
	/// Verwerkt het /Employees/Create formulier. Maakt eerst een ApplicationUser (inlogaccount) aan met rol 'Employee' en koppelt deze vervolgens aan een nieuwe Employee entry in de database.
	/// </summary>
	/// <param name="model">Verwijst naar een EmployeeCreateViewModel instantie met de ingevulde formuliergegevens.</param>
	/// <returns>/Views/Employees/Index of /Views/Employees/Create indien er een fout is opgetreden.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(EmployeeCreateViewModel model)
	{
		if (ModelState.IsValid)
		{
			// Maak het Identity inlogaccount aan
			var newUser = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				EmailConfirmed = true,
				CreatedAt = DateTime.UtcNow
			};

			// Usermanager geeft automatisch het wachtwoord 'Welcome123!' aan de newUser
			var result = await _userManager.CreateAsync(newUser, "Welcome123!");

			if (result.Succeeded)
			{
				// newUser krijgt de role 'Employee'
				await _userManager.AddToRoleAsync(newUser, "Employee");

				// De aangemaakte gegevens worden gekoppeld aan de nieuwe employee
				var employee = new Employee
				{
					ApplicationUserId = newUser.Id,
					EmployeeNumber = model.EmployeeNumber,
					JobTitle = model.JobTitle,
					HourlyWage = model.HourlyWage,
					HireDate = model.HireDate,
					DateOfBirth = model.DateOfBirth,
					Street = model.Street,
					PostalCode = model.PostalCode,
					City = model.City,
					Notes = model.Notes,
					CreatedAt = DateTime.UtcNow
				};

				_context.Employees.Add(employee);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}


			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		return View(model);
	}

	// GET: EMPLOYEES/Edit/{Id}
	/// <summary>
	/// Toont een formulier om een bestaande Employee en de daaraan gekoppelde ApplicationUser (inloggegevens) te bewerken.
	/// </summary>
	/// <param name="id">Verwijst naar het Id in de Employees database-tabel van de specifieke werknemer die werd opgehaald voor het bewerk-formulier.</param>
	/// <returns>/Views/Employees/Index of een 404NotFound-pagina indien de werknemer niet bestaat.</returns>
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null) return NotFound();

		// Laadt de gegevens van de employee samen met de gekoppelde inloggegevens
		var employee = await _context.Employees
			.Include(e => e.User)
			.FirstOrDefaultAsync(m => m.Id == id);

		if (employee == null || employee.IsDeleted) return NotFound();

		// vm laadt de waarden van de employee
		var vm = new EmployeeEditViewModel
		{
			Id = employee.Id,
			ApplicationUserId = employee.ApplicationUserId,
			EmployeeNumber = employee.EmployeeNumber,
			FirstName = employee.User?.FirstName ?? string.Empty,
			LastName = employee.User?.LastName ?? string.Empty,
			JobTitle = employee.JobTitle ?? string.Empty,
			HourlyWage = employee.HourlyWage,
			HireDate = employee.HireDate,
			DateOfBirth = employee.DateOfBirth,
			Street = employee.Street,
			PostalCode = employee.PostalCode,
			City = employee.City,
			Notes = employee.Notes
		};

		return View(vm);
	}

	// POST: EMPLOYEES/Edit/{Id}
	/// <summary>
	/// Verwerkt /Employees/Edit/{Id} om zowel de Employee als de gekoppelde ApplicationUser te bewerken. Past het UpdatedAt veld aan.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Employees database-tabel van de specifieke werknemer die werd bewerkt.</param>
	/// <param name="model">Verwijst naar een EmployeeEditViewModel instantie met de bewerkbare formuliergegevens (wordt gebruikt om overposting te voorkomen).</param>
	/// <returns>/Views/Employees/Index of /Views/Employees/Edit/{Id} indien er een fout is opgetreden.</returns>	
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int? id, EmployeeEditViewModel model)
	{
		if (id != model.Id) return NotFound();

		if (ModelState.IsValid)
		{
			try
			{
				var employeeInDb = await _context.Employees
					.Include(e => e.User)
					.FirstOrDefaultAsync(e => e.Id == id);

				if (employeeInDb != null)
				{
					// Update de Identity-gegevens tijdens het bewerken van de employee
					if (employeeInDb.User != null)
					{
						employeeInDb.User.FirstName = model.FirstName;
						employeeInDb.User.LastName = model.LastName;
					}

					// Update de Employee-gegevens
					employeeInDb.JobTitle = model.JobTitle;
					employeeInDb.HourlyWage = model.HourlyWage;
					employeeInDb.HireDate = model.HireDate;
					employeeInDb.DateOfBirth = model.DateOfBirth;
					employeeInDb.Street = model.Street;
					employeeInDb.PostalCode = model.PostalCode;
					employeeInDb.City = model.City;
					employeeInDb.Notes = model.Notes;
					employeeInDb.UpdatedAt = DateTime.UtcNow;

					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!EmployeeExists(model.Id)) return NotFound();
				else throw;
			}
		}

		return View(model);
	}

	// GET: EMPLOYEES/Delete/{Id}
	/// <summary>
	/// Toont een bevestigingspagina voor het verwijderen (soft-delete) van een specifieke Employee.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van de werknemer die verwijderd moet worden.</param>
	/// <returns>/Views/Employees/Delete/{Id} of een 404NotFound-pagina indien de werknemer niet bestaat.</returns>
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(m => m.Id == id);
		if (employee == null)
		{
			return NotFound();
		}

		return View(employee);
	}

	// POST: EMPLOYEES/Delete/{Id}
	/// <summary>
	/// Verwerkt /Employees/Delete/{Id} om een Employee te verwijderen (soft-delete). Past de IsDeleted, DeletedAt & DeletedReason velden aan.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van de werknemer die verwijderd moet worden.</param>
	/// <returns>Een redirect naar /Views/Employees/Index.</returns>
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int? id)
	{
		var employee = await _context.Employees.FindAsync(id);
		if (employee != null)
		{
			employee.IsDeleted = true;
			employee.DeletedAt = DateTime.UtcNow;
			employee.DeletedReason = "Verwijderd voor administratieve redenen";
			_context.Update(employee);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool EmployeeExists(int? id)
	{
		return _context.Employees.Any(e => e.Id == id);
	}
}
