using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Data
{
	public class GreenManagerDbContext : IdentityDbContext<ApplicationUser>
	{
	}
}
