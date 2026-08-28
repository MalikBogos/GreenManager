using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace GreenManager_Web.Controllers.Api
{
	/// <summary>
	/// REST API Controller voor CRUD-operations op materiaal (Materials) voor de MAUI applicatie, beveiligd met JWT tokens. Enkel gebruikers met de rol 'Admin' en 'Employee' mogen deze acties uitvoeren. Enkel gebruikers met de rol 'Admin' mogen Deletes uitvoeren.
	/// </summary>
	[Authorize(Policy = "EmployeeAccess")]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class MaterialsController : ControllerBase
	{
		private readonly GreenManagerDbContext _context;

		public MaterialsController(GreenManagerDbContext context)
		{
			_context = context;
		}

		// GET: api/Materials
		/// <summary>
		/// Haalt al het niet soft-deleted materiaal op.
		/// </summary>
		/// <returns>200 OK met een lijst van Materials.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
		{
			return await _context.Materials.Where(m => !m.IsDeleted).ToListAsync();
		}

		// POST: api/Materials
		/// <summary>
		/// Maakt een nieuw materiaal aan.
		/// </summary>
		/// <param name="material">De materiaalgegevens om aan te maken.</param>
		/// <returns>201 Created met het aangemaakte materiaal inclusief zijn toegekende Id.</returns>
		[HttpPost]
		public async Task<ActionResult<Material>> PostMaterial(Material material)
		{
			_context.Materials.Add(material);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetMaterials), new { id = material.Id }, material);
		}

		// PUT: api/Materials/{Id}
		/// <summary>
		/// Werkt een bestaand materiaal bij. Past UpdatedAt aan.
		/// </summary>
		/// <param name="id">Het Id van het materiaal dat bijgewerkt wordt.</param>
		/// <param name="material">De bijgewerkte materiaalgegevens; material.Id moet overeenkomen met id>.</param>
		/// <returns>204 No Content bij succes, 400 Bad Request als de Id's niet overeenkomen.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> PutMaterial(int id, Material material)
		{
			if (id != material.Id) return BadRequest();

			material.UpdatedAt = DateTime.UtcNow;
			_context.Entry(material).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// DELETE: api/Materials/{Id}
		/// <summary>
		/// Verwijdert (soft-delete) een materiaal. Enkel toegankelijk voor gebruikers met de rol Admin.
		/// </summary>
		/// <param name="id">Het Id van het materiaal dat verwijderd wordt.</param>
		/// <returns>204 No Content bij succes, 404 Not Found als het materiaal niet bestaat.</returns>
		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMaterial(int id)
		{
			var material = await _context.Materials.FindAsync(id);
			if (material == null) return NotFound();

			// Soft-delete
			material.IsDeleted = true;
			material.DeletedAt = DateTime.UtcNow;
			material.DeletedReason = "Verwijderd via de mobiele app";

			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}