
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Data;

public class ProjectsController : Controller
{
    private readonly GreenManagerDbContext _context;

    public ProjectsController(GreenManagerDbContext context)
    {
        _context = context;
    }

    // GET: PROJECTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Projects.ToListAsync());
    }

    // GET: PROJECTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects
            .FirstOrDefaultAsync(m => m.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // GET: PROJECTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PROJECTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,Status,CustomerId,Customer,ProjectAddress,Budget,ProjectEmployees,ProjectMaterials,WorkLogs,Notes,Id,CreatedAt,UpdatedAt,IsDeleted,DeletedReason,DeletedAt")] Project project)
    {
        if (ModelState.IsValid)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(project);
    }

    // GET: PROJECTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        return View(project);
    }

    // POST: PROJECTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Name,Description,StartDate,EndDate,Status,CustomerId,Customer,ProjectAddress,Budget,ProjectEmployees,ProjectMaterials,WorkLogs,Notes,Id,CreatedAt,UpdatedAt,IsDeleted,DeletedReason,DeletedAt")] Project project)
    {
        if (id != project.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.Id))
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
        return View(project);
    }

    // GET: PROJECTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects
            .FirstOrDefaultAsync(m => m.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // POST: PROJECTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProjectExists(int? id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }
}
