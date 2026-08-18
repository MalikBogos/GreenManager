using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Models.Data;
using Models.Entities;

namespace GreenManager_Web.Controllers
{
	public abstract class BaseController : Controller
	{
		protected readonly GreenManagerDbContext _context;
		protected readonly UserManager<ApplicationUser> _userManager;
		protected readonly IStringLocalizer<SharedResource> _localizer;

		public BaseController(GreenManagerDbContext context, UserManager<ApplicationUser> userManager, IStringLocalizer<SharedResource> localizer)
		{
			_context = context;
			_userManager = userManager;
			_localizer = localizer;
		}
	}
}
