using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Data;

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
		/// GET: api/customersapi
		/// Haalt een lijst op van alle actieve klanten in JSON formaat
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetCustomers()
		{
			// Haal actieve klanten op uit de database
			var customers = await _context.Customers
				.Where(c => !c.IsDeleted)
				// Maakt een 'anoniem object' aan met alleen de nodige gegevens voor de MAUI app om JSON crashes te voorkomen 
				.Select(c => new
				{
					c.Id,
					c.FirstName,
					c.LastName,
					c.Email,
					c.PhoneNumber,
					c.City
				})
				.ToListAsync();

			// Ok() zorgt ervoor dat de lijst wordt omgezet in een HTTP 200 JSON-antwoord
			return Ok(customers);
		}

		/// <summary>
		/// GET: api/customersapi/5
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
	}
}