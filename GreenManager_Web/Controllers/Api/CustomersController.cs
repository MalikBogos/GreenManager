using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace GreenManager_Web.Controllers.Api
{
	/// <summary>
	/// REST API Controller voor CRUD-operations op klanten (Customers) voor de MAUI applicatie, beveiligd met JWT tokens. Enkel gebruikers met de rol 'Admin' en 'Employee' mogen deze acties uitvoeren. Enkel gebruikers met de rol 'Admin' mogen Deletes uitvoeren.
	/// </summary>
	[Authorize(Policy = "EmployeeAccess")]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class CustomersController : ControllerBase
	{
		private readonly GreenManagerDbContext _context;

		public CustomersController(GreenManagerDbContext context)
		{
			_context = context;
		}

		// GET: api/Customers
		/// <summary>
		/// Haalt alle niet soft-deleted klanten (Customers) op.
		/// </summary>
		/// <returns>200 OK met een lijst van Customers.</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
		{
			// We halen alleen de klanten op die niet (soft) deleted zijn
			return await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
		}

		// GET: api/Customers/{Id}
		/// <summary>
		/// Haalt de details van een specifieke, niet soft-deleted klant op.
		/// </summary>
		/// <param name="id">Het Id van de klant die opgehaald wordt.</param>
		/// <returns>200 OK met de klantgegevens of 404 Not Found als de klant niet bestaat.</returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetCustomer(int id)
		{
			var customer = await _context.Customers
				.Where(c => !c.IsDeleted && c.Id == id)
				.Select(c => new
				{
					c.Id,
					c.FirstName,
					c.LastName,
					c.CompanyName,
					c.Email,
					c.PhoneNumber,
					c.Street,
					c.PostalCode,
					c.City
				})
				.FirstOrDefaultAsync();

			if (customer == null)
			{
				return NotFound(); // Geeft een HTTP 404 JSON antwoord
			}

			return Ok(customer);
		}

		// POST: api/Customers
		/// <summary>
		/// Maakt een nieuwe klant aan.
		/// </summary>
		/// <param name="customer">De klantgegevens om aan te maken.</param>
		/// <returns>201 Created met de aangemaakte klant inclusief zijn toegekende Id.</returns>
		[HttpPost]
		public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
		{
			_context.Customers.Add(customer);
			await _context.SaveChangesAsync();

			// Dit stuurt een '201 Created' succescode terug naar MAUI, inclusief de nieuwe ID
			return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
		}

		// PUT: api/Customers/{Id}
		/// <summary>
		/// Werkt een bestaande klant bij. Past UpdatedAt aan.
		/// </summary>
		/// <param name="id">Het Id van de klant die bijgewerkt wordt.</param>
		/// <param name="customer">De bijgewerkte klantgegevens; customer.Id moet overeenkomen met id.</param>
		/// <returns>204 No Content bij succes, 400 Bad Request als de Id's niet overeenkomen, 404 Not Found als de klant niet meer bestaat.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> PutCustomer(int id, Customer customer)
		{
			if (id != customer.Id)
			{
				return BadRequest("Het ID komt niet overeen.");
			}

			customer.UpdatedAt = DateTime.UtcNow;
			_context.Entry(customer).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CustomerExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// DELETE: api/Customers/{Id}
		/// <summary>
		/// Verwijdert (soft-delete) een klant. Enkel toegankelijk voor gebruikers met de rol Admin.
		/// </summary>
		/// <param name="id">Het Id van de klant die verwijderd wordt.</param>
		/// <returns>204 No Content bij succes, 404 Not Found als de klant niet bestaat.</returns>
		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCustomer(int id)
		{
			var customer = await _context.Customers.FindAsync(id);
			if (customer == null)
			{
				return NotFound();
			}

			// Soft-delete toepassen (verwijdert niet echt uit de database)
			customer.IsDeleted = true;
			customer.DeletedAt = DateTime.UtcNow;
			customer.DeletedReason = "Verwijderd via de mobiele app";

			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool CustomerExists(int id)
		{
			return _context.Customers.Any(e => e.Id == id);
		}

		
	}
}