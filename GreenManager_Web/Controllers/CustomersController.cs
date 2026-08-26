using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

/// <summary>
/// Beheert de pagina's voor de CRUD-acties op /Customers (Klanten). Alleen gebruikers met de rol 'Admin' en 'Employee' mogen deze acties uitvoeren, anders wordt de gebruiker doorverwezen naar de 'toegang beperkt' pagina (/Accounts/AccessDenied).
/// </summary>
[Authorize(Policy = "EmployeeAccess")]
public class CustomersController : Controller
{
	private readonly GreenManagerDbContext _context;

	public CustomersController(GreenManagerDbContext context)
	{
		_context = context;
	}

	// GET: CUSTOMERS
	/// <summary>
	/// Toont een overzicht van alle, niet soft-deleted customers.
	/// </summary>
	/// <returns>/Views/Customers/Index</returns>
	public async Task<IActionResult> Index()
	{
		var activeCustomers = await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();

		return View(activeCustomers);
	}

	// GET: CUSTOMERS/Details/{Id}
	/// <summary>
	/// Toont een overzicht met alle details van een specifiek, niet soft-deleted Customer.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Customers database-tabel van de specifieke Customer die werd opgehaald.</param>
	/// <returns>/Views/Customers/Details/{Id} of een 404NotFound-pagina indien de Customer niet bestaat.</returns>
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var customer = await _context.Customers
			.FirstOrDefaultAsync(m => m.Id == id);
		if (customer == null)
		{
			return NotFound();
		}

		return View(customer);
	}

	// GET: CUSTOMERS/Create
	/// <summary>
	/// Toont een formulier om een nieuwe Customer aan te maken.
	/// </summary>
	/// <returns>/Views/Customers/Create met een Customer aanmaak-formulier.</returns>
	public IActionResult Create()
	{
		return View();
	}

	// POST: CUSTOMERS/Create
	/// <summary>
	/// Verwerkt het /Customers/Create formulier om een nieuwe Customer aan te maken.
	/// </summary>
	/// <param name="customer">Verwijst naar een Customer instantie met de ingevulde formuliergegevens. Gebruikt [Bind] om enkel de gegevens door te geven die nodig zijn (wordt gebruikt om overposting te voorkomen).</param>
	/// <returns>/Views/Customers/Index of /Views/Customers/Create indien er een fout is opgetreden.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("FirstName,LastName,CompanyName,VATNumber,Notes,Email,PhoneNumber,Street,PostalCode,City")] Customer customer)
	{
		if (ModelState.IsValid)
		{
			_context.Add(customer);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(customer);
	}

	// GET: CUSTOMERS/Edit/{Id}
	/// <summary>
	/// Toont een formulier om een bestaande Customer te bewerken.
	/// </summary>
	/// <param name="id">Verwijst naar het Id in de Customers database-tabel van de specifieke Customer dat werd opgehaald voor het bewerk-formulier.</param>
	/// <returns>/Views/Customers/Edit/{Id} of een 404NotFound-pagina indien de Customer niet bestaat.</returns>
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var customer = await _context.Customers.FindAsync(id);
		if (customer == null)
		{
			return NotFound();
		}
		return View(customer);
	}

	// POST: CUSTOMERS/Edit/{Id}
	/// <summary>
	/// Verwerkt /Customers/Edit/{Id} om een Customer te bewerken. Past het UpdatedAt veld aan.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Customers database-tabel van de specifieke Customer dat werd bewerkt.</param>
	/// <param name="customer">Verwijst naar een Customer instantie met de ingevulde formuliergegevens. Gebruikt [Bind] om enkel de gegevens door te geven die nodig zijn (wordt gebruikt om overposting te voorkomen).</param>
	/// <returns>/Views/Customers/Index of /Views/Customers/Edit/{Id} indien er een fout is opgetreden.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("FirstName,LastName,CompanyName,VATNumber,Notes,Email,PhoneNumber,Street,PostalCode,City,Id,CreatedAt,UpdatedAt")] Customer customer)
	{
		if (id != customer.Id)
		{
			return NotFound();
		}

		if (ModelState.IsValid)
		{
			try
			{
				customer.UpdatedAt = DateTime.UtcNow;
				_context.Update(customer);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CustomerExists(customer.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		return View(customer);
	}

	// GET: CUSTOMERS/Delete/{Id}
	/// <summary>
	/// Toont een bevestigingspagina die alleen toegankelijk is voor gebruikers met de rol 'Admin' voor het verwijderen (soft-delete) van een specifieke Customer.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van de Customer dat verwijderd moet worden.</param>
	/// <returns>/Views/Customers/Delete/{Id}/404NotFound-pagina indien de Customer niet bestaat of de AccessDenied-pagina indien de gebruiker niet genoeg rechten heeft.</returns>
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var customer = await _context.Customers
			.FirstOrDefaultAsync(m => m.Id == id);
		if (customer == null)
		{
			return NotFound();
		}

		return View(customer);
	}

	// POST: CUSTOMERS/Delete/{Id}
	/// <summary>
	/// Verwerkt /Customers/Delete/{Id} om een Customer te verwijderen (soft-delete). Past de IsDeleted, DeletedAt & DeletedReason velden aan.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van de Customer dat verwijderd moet worden.</param>
	/// <returns>/Views/Customers/Index.</returns>
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> DeleteConfirmed(int? id)
	{
		var customer = await _context.Customers.FindAsync(id);
		if (customer != null)
		{
			customer.IsDeleted = true;
			customer.DeletedAt = DateTime.UtcNow;
			customer.DeletedReason = "Verwijderd voor administratieve redenen";
			_context.Update(customer);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool CustomerExists(int? id)
	{
		return _context.Customers.Any(e => e.Id == id);
	}
}
