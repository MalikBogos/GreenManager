
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize(Policy = "EmployeeAccess")]
public class MaterialsController : Controller
{
    private readonly GreenManagerDbContext _context;

    public MaterialsController(GreenManagerDbContext context)
    {
        _context = context;
    }

    // GET: MATERIALS
    public async Task<IActionResult> Index()    
    {
        var activeMaterials = await _context.Materials.Where(m => !m.IsDeleted).ToListAsync();

        return View(activeMaterials);
    }

    // GET: MATERIALS/Details/5
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
    public IActionResult Create()
    {
        return View();
    }

    // POST: MATERIALS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Unit,PurchasePrice,StockQuantity,ProjectMaterials,Notes,Id,CreatedAt,UpdatedAt,IsDeleted,DeletedReason,DeletedAt")] Material material)
    {
        if (ModelState.IsValid)
        {
            _context.Add(material);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(material);
    }

    // GET: MATERIALS/Edit/5
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

    // POST: MATERIALS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Name,Description,Unit,PurchasePrice,StockQuantity,ProjectMaterials,Notes,Id,CreatedAt,UpdatedAt,IsDeleted,DeletedReason,DeletedAt")] Material material)
    {
        if (id != material.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
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

	// GET: MATERIALS/Delete/5
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

    // POST: MATERIALS/Delete/5
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
