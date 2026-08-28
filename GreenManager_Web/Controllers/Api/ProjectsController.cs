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
	/// <summary>
	/// REST API Controller voor CRUD-operations op projecten (Projects) voor de MAUI applicatie, beveiligd met JWT tokens. Enkel gebruikers met de rol 'Admin' en 'Employees' mogen deze acties uitvoeren. Enkel gebruikers met de rol 'Admin' mogen Deletes uitvoeren.
	/// </summary>
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
		/// <summary>
		/// Haalt alle niet soft-deleted projecten op, in platte DTO-vorm met de naam van de gekoppelde klant samengevoegd in 1 object.
		/// </summary>
		/// <returns>200 OK met een lijst van ProjectDto.</returns>
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
		/// <summary>
		/// Maakt een nieuw project aan.
		/// </summary>
		/// <param name="dto">De projectgegevens. dto.Status moet overeenkomen met een geldige waarde van ProjectStatus Enum.</param>
		/// <returns>200 OK bij succes, 400 Bad Request als het model ongeldig is.</returns>
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

		// PUT: api/Projects/{Id}
		/// <summary>
		/// Werkt een bestaand project bij. Past UpdatedAt aan.
		/// </summary>
		/// <param name="id">Het Id van het project dat bijgewerkt wordt.</param>
		/// <param name="dto">De bijgewerkte projectgegevens.</param>
		/// <returns>204 No Content bij succes, 400 Bad Request bij een ongeldig model, 404 Not Found als het project niet bestaat.</returns>
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

		// DELETE: api/Projects/{Id}
		/// <summary>
		/// Verwijdert (soft-delete) een project. Enkel toegankelijk voor gebruikers met de rol Admin.
		/// </summary>
		/// <param name="id">Het Id van het project dat verwijderd wordt.</param>
		/// <returns>204 No Content bij succes, 404 Not Found als het project niet bestaat.</returns>
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