using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;
using Models.Entities;

namespace GreenManager_Web.Controllers.Api
{
	/// <summary>
	/// REST API Controller voor het ophalen van klanten voor de MAUI mobiele applicatie, beveiligd met JWT tokens
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	// Hier vertellen we ASP.NET dat we JWT gebruiken voor de API, niet de website cookies
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class CustomersController : ControllerBase
	{
		private readonly GreenManagerDbContext _context;

		public CustomersController(GreenManagerDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// GET: api/Customers
		/// Haalt een lijst op van alle actieve klanten in JSON formaat
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
		{
			// We halen alleen de klanten op die niet (soft) deleted zijn
			return await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();
		}

		/// <summary>
		/// GET: api/Customers/5
		/// Haalt de details van een specifieke klant op
		/// </summary>
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

		/// <summary>
		/// PUT: api/Customers/5
		/// Past een bestaande klant aan via MAUI
		/// </summary>
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

		/// <summary>
		/// POST: api/Customers
		/// Maakt een nieuwe klant aan via MAUI
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
		{
			_context.Customers.Add(customer);
			await _context.SaveChangesAsync();

			// Dit stuurt een '201 Created' succescode terug naar MAUI, inclusief de nieuwe ID
			return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
		}


		/// <summary>
		/// DELETE: api/Customers/5
		/// Verwijdert (soft delete) een klant via MAUI
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCustomer(int id)
		{
			var customer = await _context.Customers.FindAsync(id);
			if (customer == null)
			{
				return NotFound();
			}

			// Soft-delete toepassen (we verwijderen het niet echt uit de database)
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