
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

[Authorize(Policy = "EmployeeAccess")]
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
        var activeProjects = await _context.Projects.Include(p => p.Customer).Where(p => !p.IsDeleted).ToListAsync();

		return View(activeProjects);
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

        var project = await _context.Projects.Include(p => p.Customer)
		.Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employee).ThenInclude(e => e.User)
		.Include(p => p.ProjectMaterials).ThenInclude(pm => pm.Material)
		.Include(p => p.WorkLogs).ThenInclude(wl => wl.Employee).ThenInclude(e => e.User).FirstOrDefaultAsync(m => m.Id == id);

        if (project == null || project.IsDeleted)
        {
            return NotFound();
        }

		ViewBag.Customers = new SelectList(_context.Customers.Where(c => !c.IsDeleted), "Id", "LastName", project.CustomerId);
		ViewBag.Materials = new SelectList(_context.Materials.Where(m => !m.IsDeleted), "Id", "Name");

		var employees = await _context.Employees.Include(e => e.User).Where(e => !e.IsDeleted).ToListAsync();
		ViewBag.Employees = new SelectList(employees.Select(e => new { e.Id, FullName = $"{e.User.FirstName} {e.User.LastName}" }), "Id", "FullName");

		return View(project);
    }

	// POST: PROJECTS/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	// Generated
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int? id, Project projectInput)
	{
		if (id != projectInput.Id) return NotFound();

		// Negeer validatie van de sub-tabellen voor de algemene opslag
		ModelState.Remove("Customer");
		ModelState.Remove("ProjectEmployees");
		ModelState.Remove("ProjectMaterials");
		ModelState.Remove("WorkLogs");

		if (ModelState.IsValid)
		{
			var projectInDb = await _context.Projects.FindAsync(id);
			if (projectInDb != null)
			{
				projectInDb.Name = projectInput.Name;
				projectInDb.Description = projectInput.Description;
				projectInDb.StartDate = projectInput.StartDate;
				projectInDb.EndDate = projectInput.EndDate;
				projectInDb.Status = projectInput.Status;
				projectInDb.ProjectAddress = projectInput.ProjectAddress;
				projectInDb.Budget = projectInput.Budget;
				projectInDb.Notes = projectInput.Notes;
				projectInDb.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
		}
		return RedirectToAction(nameof(Edit), new { id = id });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddProjectEmployee(int ProjectId, int EmployeeId, DateTime PlannedDate, decimal EstimatedHours)
	{
		_context.ProjectEmployees.Add(new ProjectEmployee { ProjectId = ProjectId, EmployeeId = EmployeeId, PlannedDate = PlannedDate, EstimatedHours = EstimatedHours });
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Edit), new { id = ProjectId });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddProjectMaterial(int ProjectId, int MaterialId, decimal Quantity)
	{
		var material = await _context.Materials.FindAsync(MaterialId);
		if (material != null && Quantity <= material.StockQuantity)
		{
			material.StockQuantity -= Quantity; // Verminder voorraad
			_context.ProjectMaterials.Add(new ProjectMaterial { ProjectId = ProjectId, MaterialId = MaterialId, Quantity = Quantity });
			await _context.SaveChangesAsync();
		}
		return RedirectToAction(nameof(Edit), new { id = ProjectId });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddWorkLog(int ProjectId, int EmployeeId, DateTime WorkDate, decimal HoursWorked, string TaskDescription)
	{
		var employee = await _context.Employees.FindAsync(EmployeeId);
		if (employee != null)
		{
			_context.WorkLogs.Add(new WorkLog { ProjectId = ProjectId, EmployeeId = EmployeeId, WorkDate = WorkDate, HoursWorked = HoursWorked, TaskDescription = TaskDescription, HourlyWageAtTime = employee.HourlyWage });
			await _context.SaveChangesAsync();
		}
		return RedirectToAction(nameof(Edit), new { id = ProjectId });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteSubItem(int id, int projectId, string type)
	{
		if (type == "Material")
		{
			var item = await _context.ProjectMaterials.FindAsync(id);
			if (item != null) { item.IsDeleted = true; var mat = await _context.Materials.FindAsync(item.MaterialId); if (mat != null) mat.StockQuantity += item.Quantity; }
		}
		else if (type == "Employee") { var item = await _context.ProjectEmployees.FindAsync(id); if (item != null) item.IsDeleted = true; }
		else if (type == "WorkLog") { var item = await _context.WorkLogs.FindAsync(id); if (item != null) item.IsDeleted = true; }

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Edit), new { id = projectId });
	}



	// GET: PROJECTS/Delete/5
	[Authorize(Policy = "AdminOnly")]
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
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            project.IsDeleted = true;
            project.DeletedAt = DateTime.UtcNow;
            project.DeletedReason = "Verwijderd voor administratieve redenen";
            _context.Update(project);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProjectExists(int? id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }
}
