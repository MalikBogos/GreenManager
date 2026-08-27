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
	// Dit vertelt het systeem dat dit een API is
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;

		public AuthenticationController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_configuration = configuration;
		}

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

			// Controleer het wachtwoord
			if (await _userManager.CheckPasswordAsync(user, model.Password))
			{

				if (user.IsBlocked || user.IsDeleted)
					return Unauthorized(new { message = "Dit account is geblokkeerd of verwijderd." });



				// Maak het JWT-token aan
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

			// Als inloggen mislukt, geef een "Niet Geautoriseerd" foutmelding (401)
			return Unauthorized(new { message = "E-mailadres of wachtwoord is onjuist." });
		}
	}
}