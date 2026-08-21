using GreenManager_Web.Middleware;
using GreenManager_Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Models.Data;
using Models.Entities;
using Serilog;
using System.Text;

namespace GreenManager_Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Gebruik serilog voor logging, textbestand in /Logs
			Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).WriteTo.Console().WriteTo.File("Logs/info-log.txt", rollingInterval: RollingInterval.Day).CreateLogger();

			builder.Host.UseSerilog();

			// Database connection
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connectionstring Defaultconnection no found");

			// Identity setup

			builder.Services.AddDbContext<GreenManagerDbContext>(options =>
				options.UseSqlServer(connectionString));

			// Wachtwoordinstellingen voor gebruikers
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredLength = 3;
				options.Password.RequireDigit = false;
				options.SignIn.RequireConfirmedEmail = true;

			})
			.AddEntityFrameworkStores<GreenManagerDbContext>()
			.AddDefaultTokenProviders();


			

			// Gebruikt voor beperking van toegang tot bepaalde rollen
			builder.Services.AddAuthorization(options =>
			{
				// Enkel toegankelijk voor gebruikers met rol 'Admin'
				options.AddPolicy("AdminOnly", policy =>
				policy.RequireRole("Admin"));

				// Toegankelijk voor zowel Admin als Employee
				options.AddPolicy("EmployeeAccess", policy =>
				policy.RequireRole("Admin", "Employee"));

				// Toegankelijk voor alle rollen
				options.AddPolicy("GuestAccess", policy =>
				policy.RequireRole("Admin", "Employee", "Guest"));
			});


			// Cookie-instelling
			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Accounts/Login";
				options.AccessDeniedPath = "/Accounts/AccessDenied";
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
				options.SlidingExpiration = true;
			});

			// Localisation toevoegen, zoekt in folder /Resources
			builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

			var supportedCultures = new[] { "nl", "en", "fr" };

			builder.Services.Configure<RequestLocalizationOptions>(options =>
			{
				options.SetDefaultCulture(supportedCultures[0])
					.AddSupportedCultures(supportedCultures)
					.AddSupportedUICultures(supportedCultures);

				options.RequestCultureProviders = new List<IRequestCultureProvider>
			{
				new CookieRequestCultureProvider()   // taal wordt in een cookie onthouden
			};
			});

			// Gebruik van email confirmation
			builder.Services.AddTransient<IEmailSender, EmailSender>();

			// Add services to the container. (+ AddRazorRuntimeCompilation)
			builder.Services.AddControllersWithViews()
			.AddRazorRuntimeCompilation()
			.AddViewLocalization()
			.AddDataAnnotationsLocalization(options =>
			{
				options.DataAnnotationLocalizerProvider = (type, factory) =>
					factory.Create(typeof(SharedResource));
			});


			// Nodig voor gebruik van scaffolding
			builder.Services.AddRazorPages();

			// MAUI koppeling
			builder.Services.AddEndpointsApiExplorer();

			// Default Swagger configuratie
			//builder.Services.AddSwaggerGen();

			// JWT
			builder.Services.AddAuthentication()
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
				};
			});

			// Swagger configuratie voor het testen van JWT tokens
			builder.Services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Plak hieronder je JWT token die je verkreeg na succesvolle login"
				});

				options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
				{
					[new OpenApiSecuritySchemeReference("bearer", document)] = []
				});
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			} else
			{
				// Swagger documentatie-pagina
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			// Ajax?

			app.UseRouting();

			// Localisation
			app.UseRequestLocalization(app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);

			// Activatie inlog + rechten
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapStaticAssets();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}")
				.WithStaticAssets();

			app.MapRazorPages(); // scaffolding

			// Middleware
			app.Run();
		}
	}
}
