using GreenManager_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GreenManager_Web.Controllers.Api
{
	/// <summary>
	/// REST API Controller voor het aanmelden bij de MAUI app. Maakt bij een succesvolle aanmelding een JWT-token aan.
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Dependency Injection van ASP.NET Identity en de configuratie voor JWT instellingen.
		/// </summary>
		/// <param name="userManager">De ASP.NET Identity UserManager voor gebruikers en wachtwoordcontrole.</param>
		/// <param name="configuration">De applicatieconfiguratie, gebruikt om de JWT-instellingen op te halen.</param>
		public AuthenticationController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_configuration = configuration;
		}

		/// <summary>
		/// Verifieert het emailadres en wachtwoord van een gebruiker en maakt een JWT-token aan met gebruikersnaam en rollen. Het JWT-token blijft 3 uur geldig.
		/// </summary>
		/// <param name="model">Verwijst naar een instantie van LoginViewModel met emailadres, wachtwoord en rememberme.</param>
		/// <returns>200 OK met het JWT-token en vervaldatum indien geslaagd, anders een foutmelding.</returns>
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel model)
		{
			// Zoek de gebruiker in de database
			var user = await _userManager.FindByEmailAsync(model.Email);


			if (user == null)
			{
				return Unauthorized(new { message = "Emailadres of wachtwoord is onjuist" });
			}

			var userRoles = await _userManager.GetRolesAsync(user);

			// Controleert het wachtwoord
			if (await _userManager.CheckPasswordAsync(user, model.Password))
			{

				if (user.IsBlocked || user.IsDeleted)
					return Unauthorized(new { message = "Dit account is geblokkeerd of verwijderd." });



				// Stelt de Claims in
				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName ?? ""),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				};

				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				// Haal de JWT Key op uit de usersecrets/appsettings.json
				var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));

				// Maak een nieuw token aan
				var token = new JwtSecurityToken(
					issuer: _configuration["Jwt:Issuer"],
					audience: _configuration["Jwt:Audience"],
					expires: DateTime.Now.AddHours(3),
					claims: authClaims,
					signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
				);

				// Stuur het token terug naar de MAUI app
				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					expiration = token.ValidTo
				});
			}

			// Als inloggen mislukt, geef een "Niet Geautoriseerd" foutmelding (401).
			return Unauthorized(new { message = "E-mailadres of wachtwoord is onjuist." });
		}
	}
}