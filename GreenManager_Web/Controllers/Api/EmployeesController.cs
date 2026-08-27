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