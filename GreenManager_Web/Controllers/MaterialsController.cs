
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Data;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Beheert de pagina's voor de CRUD-acties op /Materials (Materialen). Alleen gebruikers met de rol 'Admin' en 'Employee' mogen deze acties uitvoeren, anders wordt de gebruiker doorverwezen naar de 'toegang beperkt' pagina (/Accounts/AccessDenied).
/// </summary>
[Authorize(Policy = "EmployeeAccess")]
public class MaterialsController : Controller
{
    private readonly GreenManagerDbContext _context;

    public MaterialsController(GreenManagerDbContext context)
    {
        _context = context;
    }

	// GET: MATERIALS
	/// <summary>
	/// Toont een overzicht van alle, niet soft-deleted Materialen.
	/// </summary>
	/// <returns>/Views/Materials/Index met een overzicht van actieve materialen.</returns>
	public async Task<IActionResult> Index()    
    {
        var activeMaterials = await _context.Materials.Where(m => !m.IsDeleted).ToListAsync();

        return View(activeMaterials);
    }

	// GET: MATERIALS/Details/{Id}
	/// <summary>
	/// Toont een overzicht met alle details van een specifiek, niet soft-deleted Material.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Materials database-tabel van het specifieke Material dat werd opgehaald.</param>
	/// <returns>/Views/Materials/Details/{Id} of een 404NotFound-pagina indien het Material niet bestaat.</returns>
	public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.Id == id);
        if (material == null)
        {
            return NotFound();
        }

        return View(material);
    }

	// GET: MATERIALS/Create
	/// <summary>
	/// Toont een formulier om een nieuw Material aan te maken.
	/// </summary>
	/// <returns>/Views/Materials/Create met een Material aanmaak-formulier.</returns>
	public IActionResult Create()
    {
        return View();
    }

	// POST: MATERIALS/Create
	/// <summary>
	/// Verwerkt het /Materials/Create formulier om een nieuw Material aan te maken.
	/// </summary>
	/// <param name="material">Verwijst naar een Material instantie met de ingevulde formuliergegevens. Gebruikt [Bind] om enkel de gegevens door te geven die nodig zijn (wordt gebruikt om overposting te voorkomen).</param>
	/// <returns>/Views/Materials/Index of /Views/Materials/Create indien er een validatiefout is opgetreden.</returns>
	[HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Unit,PurchasePrice,StockQuantity,ProjectMaterials,Notes")] Material material)
    {
        if (ModelState.IsValid)
        {
            _context.Add(material);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(material);
    }

	// GET: MATERIALS/Edit/{Id}
	/// <summary>
	/// Toont een formulier om een bestaand Material te bewerken.
	/// </summary>
	/// <param name="id">Verwijst naar het Id in de Materials database-tabel van het specifieke materiaal dat werd opgehaald voor het bewerk-formulier.</param>
	/// <returns>/Views/Materials/Edit/{Id} of een 404NotFound-pagina indien het materiaal niet bestaat.</returns>
	public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var material = await _context.Materials.FindAsync(id);
        if (material == null)
        {
            return NotFound();
        }
        return View(material);
    }

	// POST: MATERIALS/Edit/{Id}
	/// <summary>
	/// Verwerkt /Materials/Edit/{Id} om een Material te bewerken. Past het UpdatedAt veld aan.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Materials database-tabel van het specifieke materiaal dat werd bewerkt.</param>
	/// <param name="material">Verwijst naar een Material instantie met de ingevulde formuliergegevens. Gebruikt [Bind] om overposting te voorkomen.</param>
	/// <returns>/Views/Materials/Index of /Views/Materials/Edit/{Id} indien er een validatiefout is opgetreden.</returns>
	[HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Name,Description,Unit,PurchasePrice,StockQuantity,ProjectMaterials,Notes,Id,CreatedAt,UpdatedAt")] Material material)
    {
        if (id != material.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                material.UpdatedAt = DateTime.UtcNow;
                _context.Update(material);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(material.Id))
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
        return View(material);
    }

	// GET: MATERIALS/Delete/{Id}
	/// <summary>
	/// Toont een bevestigingspagina die alleen toegankelijk is voor gebruikers met de rol 'Admin' voor het verwijderen (soft-delete) van een specifiek Material.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van het Material dat verwijderd moet worden.</param>
	/// <returns>/Views/Materials/Delete/{Id} of een 404NotFound-pagina indien het materiaal niet bestaat (of AccessDenied-pagina bij onvoldoende rechten).</returns>
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.Id == id);
        if (material == null)
        {
            return NotFound();
        }

        return View(material);
    }

	// POST: MATERIALS/Delete/{Id}
	/// <summary>
	/// Verwerkt /Materials/Delete/{Id} om een Material te verwijderen (soft-delete). Past de IsDeleted, DeletedAt & DeletedReason velden aan.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van het Material dat verwijderd moet worden.</param>
	/// <returns>/Views/Materials/Index.</returns>
	[HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var material = await _context.Materials.FindAsync(id);
        if (material != null)
        {
            material.IsDeleted = true;
            material.DeletedAt = DateTime.UtcNow;
            material.DeletedReason = "Verwijderd voor administratieve redenen";
            _context.Update(material);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MaterialExists(int? id)
    {
        return _context.Materials.Any(e => e.Id == id);
    }
}
