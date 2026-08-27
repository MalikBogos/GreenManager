using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace GreenManager_Web.Controllers.Api
{
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
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
		{
			return await _context.Materials.Where(m => !m.IsDeleted).ToListAsync();
		}

		// POST: api/Materials
		[HttpPost]
		public async Task<ActionResult<Material>> PostMaterial(Material material)
		{
			_context.Materials.Add(material);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetMaterials), new { id = material.Id }, material);
		}

		// PUT: api/Materials/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutMaterial(int id, Material material)
		{
			if (id != material.Id) return BadRequest();

			material.UpdatedAt = DateTime.UtcNow;
			_context.Entry(material).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// DELETE: api/Materials/5
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