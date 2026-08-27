using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.DTOs;
using Models.Entities;
using Models.Enums;

namespace GreenManager_Web.Controllers.Api
{
	[Authorize(Policy = "EmployeeAccess")]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class ProjectsController : ControllerBase
	{
		private readonly GreenManagerDbContext _context;

		public ProjectsController(GreenManagerDbContext context)
		{
			_context = context;
		}

		// GET: api/Projects
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
		{
			var projects = await _context.Projects
				.Include(p => p.Customer)
				.Where(p => !p.IsDeleted)
				.Select(p => new ProjectDto
				{
					Id = p.Id,
					Name = p.Name,
					Description = p.Description,
					StartDate = p.StartDate,
					EndDate = p.EndDate,
					Status = p.Status.ToString(),
					ProjectAddress = p.ProjectAddress,
					Budget = p.Budget,
					Notes = p.Notes,
					CustomerId = p.CustomerId,
					CustomerFirstName = p.Customer != null ? p.Customer.FirstName : string.Empty,
					CustomerLastName = p.Customer != null ? p.Customer.LastName : string.Empty
				})
				.ToListAsync();

			return Ok(projects);
		}

		// POST: api/Projects
		[HttpPost]
		public async Task<IActionResult> PostProject([FromBody] ProjectRequestDto dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var project = new Project
			{
				Name = dto.Name,
				Description = dto.Description,
				StartDate = dto.StartDate,
				EndDate = dto.EndDate,
				Status = Enum.Parse<ProjectStatus>(dto.Status), 
				CustomerId = dto.CustomerId,
				ProjectAddress = dto.ProjectAddress,
				Budget = dto.Budget,
				Notes = dto.Notes,
				CreatedAt = DateTime.UtcNow
			};

			_context.Projects.Add(project);
			await _context.SaveChangesAsync();

			return Ok(); // Status 200 OK
		}

		// PUT: api/Projects/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutProject(int id, [FromBody] ProjectRequestDto dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var project = await _context.Projects.FindAsync(id);
			if (project == null) return NotFound();

			project.Name = dto.Name;
			project.Description = dto.Description;
			project.StartDate = dto.StartDate;
			project.EndDate = dto.EndDate;
			project.Status = Enum.Parse<ProjectStatus>(dto.Status);
			project.CustomerId = dto.CustomerId;
			project.ProjectAddress = dto.ProjectAddress;
			project.Budget = dto.Budget;
			project.Notes = dto.Notes;
			project.UpdatedAt = DateTime.UtcNow;

			_context.Entry(project).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// DELETE: api/Projects/5
		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProject(int id)
		{
			var project = await _context.Projects.FindAsync(id);
			if (project == null) return NotFound();

			// Voer een soft-delete uit
			project.IsDeleted = true;
			project.DeletedAt = DateTime.UtcNow;
			project.DeletedReason = "Verwijderd via de mobiele app";

			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}