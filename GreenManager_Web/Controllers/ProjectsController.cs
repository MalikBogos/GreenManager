using GreenManager_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

/// <summary>
/// Beheert de pagina's voor de CRUD-acties op /projects. Alleen gebruikers met de rol 'Admin' en 'Employee' mogen deze acties uitvoeren, anders wordt de gebruiker doorverwezen naar de 'toegang beperkt' pagina (/Accounts/AccessDenied).
/// </summary>
[Authorize(Policy = "EmployeeAccess")]
public class ProjectsController : Controller
{
    private readonly GreenManagerDbContext _context;

    public ProjectsController(GreenManagerDbContext context)
    {
        _context = context;
    }

    // GET: PROJECTS
	/// <summary>
	/// Toont een overzicht van alle, niet soft-deleted projecten. Include(p => p.Customer) wordt gebruikt om de klantnaam geassocieerd met het project weer te geven.
	/// </summary>
	/// <returns>/Views/Projects/Index.cshtml met een overzicht van actieve projecten.</returns>
    public async Task<IActionResult> Index()    
    {
		// Laad enkel de projecten die niet op IsDeleted = true staan;
        var activeProjects = await _context.Projects.Include(p => p.Customer).Where(p => !p.IsDeleted).ToListAsync();

		return View(activeProjects);
    }

	// GET: PROJECTS/Details/{Id}
	/// <summary>
	/// Toont een overzicht met alle details van een specifiek, niet soft-deleted project. Include(p => p.Customer) wordt gebruikt om de klantnaam geassocieerd met het project weer te geven.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Projects database-tabel van het specifieke project dat werd opgehaald.</param>
	/// <returns>/Views/Projects/Details/{Id} of een 404NotFound-pagina indien het project niet bestaat.</returns>
	public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects.Include(p => p.Customer).FirstOrDefaultAsync(m => m.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // GET: PROJECTS/Create
	/// <summary>
	/// Toont een formulier om een nieuw Project aan te maken met een keuze van niet soft-deleted Klanten om te koppelen aan het nieuw project.
	/// </summary>
	/// <returns>/Views/Projects/Create met een Project aanmaak-formulier.</returns>
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
	/// <summary>
	/// Verwerkt het /Projects/Create formulier om een nieuw Project aan te maken.
	/// </summary>
	/// <param name="model">Verwijst naar een ProjectCreateViewModel instantie met de ingevulde formuliergegevens (wordt gebruikt om overposting te voorkomen).</param>
	/// <returns>/Views/Projects/Index of /Views/Projects/Create indien er een fout is opgetreden.</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ProjectCreateViewModel model)
	{
		if (ModelState.IsValid)
		{
			// Maak een echt databank-object op basis van het ProjectCreateViewModel
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

		return View(model);
	}

	// GET: PROJECTS/Edit/{Id}
	/// <summary>
	/// Toont een formulier om een bestaand Project te bewerken en laadt alle gekoppelde data (Customer, ProjectEmployees, ProjectMaterials, WorkLogs) van het Project (.Include(p => p.?)).
	/// </summary>
	/// <param name="id">Verwijst naar het Id in de Projects database-tabel van het specifieke project dat werd opgehaald voor het bewerk-formulier.</param>
	/// <returns>/Views/Projects/Edit/{Id} of een 404NotFound-pagina indien het project niet bestaat.</returns>
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null) return NotFound();

		// Haal het Project met alle tabellen die foreignkeys hebben op 
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

	// POST: PROJECTS/Edit/
	/// <summary>
	/// Verwerkt /Projects/Edit/{Id} om een Project te bewerken. Past het UpdatedAt veld aan.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de Projects database-tabel van het specifieke project dat werd bewerkt.</param>
	/// <param name="model">Verwijst naar een ProjectEditViewModel instantie met de ingevulde formuliergegevens (wordt gebruikt om overposting te voorkomen).</param>
	/// <returns>/Views/Projects/Edit/{Id}</returns>
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

	/// <summary>
	/// Koppelt een ProjectEmployee aan een Project in het /Views/Projects/Edit/{Id} formulier en maakt hiervoor een nieuwe entry in de ProjectEmployees database-tabel met de gekoppelde gegevens.
	/// </summary>
	/// <param name="ProjectId">Verwijst naar ProjectId in de ProjectEmployees database-tabel van het specifieke project waaraan de ProjectEmployee werd gekoppeld.</param>
	/// <param name="EmployeeId">Verwijst naar EmployeeId in de ProjectEmployees database-tabel van het specifieke project waaraan de ProjectEmployee werd gekoppeld.</param>
	/// <param name="PlannedDate">Verwijst naar PlannedDate in de ProjectEmployees database-tabel van het specifieke project waaraan de ProjectEmployee werd gekoppeld.</param>
	/// <param name="EstimatedHours">Verwijst naar EstimatedHours in de ProjectEmployees database-tabel van het specifieke project waaraan de ProjectEmployee werd gekoppeld.</param>
	/// <returns>/Views/Projects/Edit/{Id}</returns>
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> AddProjectEmployee(int ProjectId, int EmployeeId, DateTime PlannedDate, decimal EstimatedHours)
	{
		_context.ProjectEmployees.Add(new ProjectEmployee { ProjectId = ProjectId, EmployeeId = EmployeeId, PlannedDate = PlannedDate, EstimatedHours = EstimatedHours });
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Edit), new { id = ProjectId });
	}

	/// <summary>
	/// Koppelt een ProjectMaterial aan een Project in het /Views/Projects/Edit/{Id} formulier, vermindert de bestaande voorraad (Materials-tabel) met het aantal dat werd toegevoegd en maakt hiervoor een nieuwe entry in de ProjectMaterials database-tabel met de gekoppelde gegevens.
	/// </summary>
	/// <param name="ProjectId">Verwijst naar ProjectId in de ProjectMaterials database-tabel van het specifieke project waaraan het ProjectMaterial werd gekoppeld.</param>
	/// <param name="MaterialId">Verwijst naar MaterialId in de ProjectMaterials database-tabel van het specifieke project waaraan het ProjectMaterial werd gekoppeld.</param>
	/// <param name="Quantity">Verwijst naar Quantity in de ProjectMaterials database-tabel van het specifieke project waaraan het ProjectMaterial werd gekoppeld.</param>
	/// <returns>/Views/Projects/Edit/{Id}</returns>
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

	/// <summary>
	/// Koppelt een WorkLog aan een Project in het /Views/Projects/Edit/{Id} formulier en maakt hiervoor een nieuwe entry in de WorkLogs database-tabel met de gekoppelde gegevens.
	/// </summary>
	/// <param name="ProjectId">Verwijst naar ProjectId in de WorkLogs database-tabel van het specifieke project waaraan de WorkLog werd gekoppeld.</param>
	/// <param name="EmployeeId">Verwijst naar EmployeeId in de WorkLogs database-tabel van het specifieke project waaraan de WorkLog werd gekoppeld.</param>
	/// <param name="WorkDate">Verwijst naar WorkDate in de WorkLogs database-tabel van het specifieke project waaraan de WorkLog werd gekoppeld.</param>
	/// <param name="HoursWorked">Verwijst naar HoursWorked in de WorkLogs database-tabel van het specifieke project waaraan de WorkLog werd gekoppeld.</param>
	/// <param name="TaskDescription">Verwijst naar TaskDescription in de WorkLogs database-tabel van het specifieke project waaraan de WorkLog werd gekoppeld.</param>
	/// <returns>/Views/Projects/Edit/{Id}</returns>
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

	/// <summary>
	/// Verwijdert (soft-delete) een sub-item (ProjectEmployee/ProjectMaterial/WorkLog) dat werd gekoppeld aan een Project. De voorraad in de Materials-tabel wordt opnieuw aangepast na de soft-delete. Past de IsDeleted, DeletedAt & DeletedReason velden aan.
	/// </summary>
	/// <param name="id">Verwijst naar Id in de database-tabel van het sub-item.</param>
	/// <param name="projectId">Verwijst naar ProjectId in de database-tabel van het sub-item.</param>
	/// <param name="type">Verwijst naar het type (ProjectMaterial/ProjectEmployee/WorkLog) sub-item.</param>
	/// <returns>/Views/Projects/Edit/{Id}</returns>
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

	// GET: PROJECTS/Delete/{Id}
	/// <summary>
	/// Toont een bevestigingspagina die alleen toegankelijk is voor gebruikers met de rol 'Admin' voor het verwijderen (soft-delete) van een specifiek Project.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van het Project dat verwijderd moet worden.</param>
	/// <returns>/Views/Projects/Delete/{Id}/404NotFound-pagina indien het project niet bestaat/de AccessDenied-pagina indien de gebruiker niet genoeg rechten heeft.</returns>
	[Authorize(Policy = "AdminOnly")]
	public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects.Include(p => p.Customer).FirstOrDefaultAsync(m => m.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // POST: PROJECTS/Delete/{Id}
	/// <summary>
	/// Verwerkt /Projects/Delete/{Id} om een Project te verwijderen (soft-delete). Past de IsDeleted, DeletedAt & DeletedReason velden aan.
	/// </summary>
	/// <param name="id">Verwijst naar het Id van het Project dat wordt verwijderd.</param>
	/// <returns>/Views/Projects/Index</returns>
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
