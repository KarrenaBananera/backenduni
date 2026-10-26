using System.Text;
using AngularApp3.Server.Configuration;
using AngularApp3.Server.Data;
using AngularApp3.Server.Data.Entities;
using AngularApp3.Server.Extensions;
using AngularApp3.Server.Identity;
using AngularApp3.Server.Repositories;
using AngularApp3.Server.Services;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection();

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt configuration section is missing.");

if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductReadRepository, ProductReadRepository>();
builder.Services.AddScoped<ICachedProductService, CachedProductService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await IdentityDataSeeder.SeedRolesAsync(app.Services);
await PromoCodeSeeder.SeedAsync(app.Services);

app.Run();
