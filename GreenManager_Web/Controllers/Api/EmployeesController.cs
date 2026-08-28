using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.DTOs;
using Models.Entities;

namespace GreenManager_Web.Controllers.Api
{
	/// <summary>
	/// REST API Controller voor CRUD-operations op werknemers (Employees) voor de MAUI applicatie, beveiligd met JWT tokens. Enkel gebruikers met de rol 'Admin' mogen deze acties uitvoeren.
	/// </summary>
	[Authorize(Policy = "AdminOnly")]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class EmployeesController : ControllerBase
	{
		private readonly GreenManagerDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public EmployeesController(GreenManagerDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: api/Employees
		/// <summary>
		/// Haalt alle niet soft-deleted werknemers op, in platte DTO-vorm met de gekoppelde gebruikersgegevens (naam, e-mail) samengevoegd in 1 object.
		/// </summary>
		/// <returns>200 OK met een lijst van EmployeeDto.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
		{
			// Koppel de user data aan de DTO
			var employees = await _context.Employees
				.Include(e => e.User)
				.Where(e => !e.IsDeleted)
				.Select(e => new EmployeeDto
				{
					Id = e.Id,
					ApplicationUserId = e.ApplicationUserId,
					FirstName = e.User.FirstName,
					LastName = e.User.LastName,
					Email = e.User.Email ?? string.Empty,
					EmployeeNumber = e.EmployeeNumber,
					HourlyWage = e.HourlyWage,
					HireDate = e.HireDate,
					DateOfBirth = e.DateOfBirth,
					JobTitle = e.JobTitle,
					Street = e.Street,
					PostalCode = e.PostalCode,
					City = e.City,
					Notes = e.Notes
				})
				.ToListAsync();

			return Ok(employees);
		}

		// POST: api/Employees
		/// <summary>
		/// Maakt een nieuwe werknemer aan. Maakt eerst een nieuwe ApplicationUser inlogaccount (met de rol Employee) via Identity en koppelt daaraan een nieuwe Employee met de specifieke Employee-gegevens.
		/// </summary>
		/// <param name="dto">De gegevens voor het nieuwe account en de nieuwe werknemer.</param>
		/// <returns>200 OK bij succes, 400 Bad Request als het model ongeldig is of het account niet aangemaakt kon worden.</returns>
		[HttpPost]
		public async Task<IActionResult> PostEmployee([FromBody] EmployeeRequestDto dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var newUser = new ApplicationUser
			{
				UserName = dto.Email,
				Email = dto.Email,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				EmailConfirmed = true,
				CreatedAt = DateTime.UtcNow
			};

			var result = await _userManager.CreateAsync(newUser, "Welcome123!");
			if (!result.Succeeded) return BadRequest(result.Errors);

			await _userManager.AddToRoleAsync(newUser, "Employee");

			var employee = new Employee
			{
				ApplicationUserId = newUser.Id,
				EmployeeNumber = dto.EmployeeNumber,
				JobTitle = dto.JobTitle,
				HourlyWage = dto.HourlyWage,
				HireDate = dto.HireDate,
				DateOfBirth = dto.DateOfBirth,
				Street = dto.Street,
				PostalCode = dto.PostalCode,
				City = dto.City,
				Notes = dto.Notes,
				CreatedAt = DateTime.UtcNow
			};

			_context.Employees.Add(employee);
			await _context.SaveChangesAsync();

			return Ok();
		}

		// PUT: api/Employees/{Id}
		/// <summary>
		/// Werkt een bestaande werknemer bij, inclusief de naamgegevens van de gekoppelde ApplicationUser. Past UpdatedAt aan.
		/// </summary>
		/// <param name="id">Het Id van de werknemer die bijgewerkt wordt.</param>
		/// <param name="dto">De bijgewerkte gegevens.</param>
		/// <returns>204 No Content bij succes, 404 Not Found als de werknemer niet bestaat.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> PutEmployee(int id, [FromBody] EmployeeRequestDto dto)
		{
			var employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
			if (employee == null) return NotFound();

			// Update user data
			employee.User.FirstName = dto.FirstName;
			employee.User.LastName = dto.LastName;

			// Update employee data
			employee.EmployeeNumber = dto.EmployeeNumber;
			employee.JobTitle = dto.JobTitle;
			employee.HourlyWage = dto.HourlyWage;
			employee.HireDate = dto.HireDate;
			employee.DateOfBirth = dto.DateOfBirth;
			employee.Street = dto.Street;
			employee.PostalCode = dto.PostalCode;
			employee.City = dto.City;
			employee.Notes = dto.Notes;
			employee.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		// DELETE: api/Employees/{Id}
		/// <summary>
		/// Verwijdert (soft-delete) een werknemer.
		/// </summary>
		/// <param name="id">Het Id van de werknemer die verwijderd wordt.</param>
		/// <returns>204 No Content bij succes, 404 Not Found als de werknemer niet bestaat.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteEmployee(int id)
		{
			var employee = await _context.Employees.FindAsync(id);
			if (employee == null) return NotFound();

			employee.IsDeleted = true;
			employee.DeletedAt = DateTime.UtcNow;
			employee.DeletedReason = "Verwijderd via de mobiele app";

			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}