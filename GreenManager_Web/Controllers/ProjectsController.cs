
using GreenManager_Web.Models;
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
		// Laad enkel de projecten die niet op IsDeleted = true staan;
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

        var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // GET: PROJECTS/Create
    public async Task<IActionResult> Create()
    {
		// Laadt een lijst met actieve klanten voor het toewijzen aan een project
		var activeCustomers = await _context.Customers
		.Where(c => !c.IsDeleted)
		.ToListAsync();

		ViewBag.Customers = new SelectList(activeCustomers, "Id", "LastName");
		return View();
    }

	// POST: PROJECTS/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ProjectCreateViewModel model)
	{
		if (ModelState.IsValid)
		{
			// Maak een echt databank-object met de gegevens uit het ProjectCreateViewModel en wijs die dan toe aan de newProject
			var newProject = new Project
			{
				Name = model.Name,
				Description = model.Description,
				StartDate = model.StartDate,
				EndDate = model.EndDate,
				Status = model.Status,
				CustomerId = model.CustomerId,
				ProjectAddress = model.ProjectAddress,
				Budget = model.Budget,
				Notes = model.Notes,
				CreatedAt = DateTime.UtcNow
			};

			_context.Projects.Add(newProject);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		// Laad de dropdown opnieuw in geval dat de POST request faalt
		var activeCustomers = await _context.Customers
			.Where(c => !c.IsDeleted)
			.OrderBy(c => c.LastName)
			.ToListAsync();

		ViewBag.Customers = new SelectList(activeCustomers, "Id", "LastName", model.CustomerId);

		return View(model);
	}

	// GET: PROJECTS/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null) return NotFound();

		// Haal project met alle tabellen die foreignkeys hebben op 
		var project = await _context.Projects
			.Include(p => p.Customer)
			.Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employee).ThenInclude(e => e.User)
			.Include(p => p.ProjectMaterials).ThenInclude(pm => pm.Material)
			.Include(p => p.WorkLogs).ThenInclude(wl => wl.Employee).ThenInclude(e => e.User)
			.FirstOrDefaultAsync(m => m.Id == id);

		if (project == null || project.IsDeleted) return NotFound();

		// Maak ViewBags voor het laden van de data in dropdowns
		ViewBag.Customers = new SelectList(_context.Customers.Where(c => !c.IsDeleted), "Id", "LastName", project.CustomerId);
		ViewBag.Materials = new SelectList(_context.Materials.Where(m => !m.IsDeleted), "Id", "Name");

		var employees = await _context.Employees.Include(e => e.User).Where(e => !e.IsDeleted).ToListAsync();
		ViewBag.Employees = new SelectList(employees.Select(e => new { e.Id, FullName = $"{e.User.FirstName} {e.User.LastName}" }), "Id", "FullName");

		var vm = new ProjectEditViewModel
		{
			Id = project.Id,
			Name = project.Name,
			Description = project.Description,
			StartDate = project.StartDate,
			EndDate = project.EndDate,
			Status = project.Status,
			CustomerId = project.CustomerId,
			ProjectAddress = project.ProjectAddress,
			Budget = project.Budget,
			Notes = project.Notes,
			CreatedAt = project.CreatedAt,
			ProjectEmployees = project.ProjectEmployees, // Geef de lijst met ProjectEmployees door aan 'vm'
			ProjectMaterials = project.ProjectMaterials, // Geef de lijst met ProjectMaterials door aan 'vm'
			WorkLogs = project.WorkLogs
		};

		return View(vm);
	}

	// POST: PROJECTS/Edit/5
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int? id, ProjectEditViewModel model)
	{
		if (id != model.Id) return NotFound();

		if (ModelState.IsValid)
		{
			var projectInDb = await _context.Projects.FindAsync(id);
			if (projectInDb != null)
			{
				// Update enkel de algemene velden
				projectInDb.Name = model.Name;
				projectInDb.Description = model.Description;
				projectInDb.StartDate = model.StartDate;
				projectInDb.EndDate = model.EndDate;
				projectInDb.Status = model.Status;
				projectInDb.ProjectAddress = model.ProjectAddress;
				projectInDb.Budget = model.Budget;
				projectInDb.Notes = model.Notes;
				projectInDb.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
		}
		return View(model);
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
		// Pas isDeleted, DeletedAt & DeletedReason aan tijdens het verwijderen
		if (type == "Material")
		{
			var item = await _context.ProjectMaterials.FindAsync(id);
			if (item != null) 
			{ 
				item.IsDeleted = true; item.DeletedAt = DateTime.UtcNow; item.DeletedReason = "Verwijderd voor administratieve redenen"; var mat = await _context.Materials.FindAsync(item.MaterialId); if (mat != null) mat.StockQuantity += item.Quantity; 
			}
		}
		else if (type == "Employee") 
		{ 
			var item = await _context.ProjectEmployees.FindAsync(id); 
			if (item != null) 
			{ 
				item.IsDeleted = true; item.DeletedAt = DateTime.UtcNow; item.DeletedReason = "Verwijderd voor administratieve redenen"; 
			} 
		}
		else if (type == "WorkLog") 
		{ 
			var item = await _context.WorkLogs.FindAsync(id); 
			if (item != null) 
			{ 
				item.IsDeleted = true; item.DeletedAt = DateTime.UtcNow; item.DeletedReason = "Verwijderd voor administratieve redenen"; 
			} 
		}

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
		// Pas IsDeleted, DeletedAt, DeletedReason aan voor soft-delete
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
