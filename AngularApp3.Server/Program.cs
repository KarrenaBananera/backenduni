using System.Security.Claims;
using System.Text;
using AngularApp3.Server.Configuration;
using AngularApp3.Server.Data;
using AngularApp3.Server.Data.Entities;
using AngularApp3.Server.Extensions;
using AngularApp3.Server.Identity;
using AngularApp3.Server.Repositories;
using AngularApp3.Server.OpenApi;
using AngularApp3.Server.Services;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            NameClaimType = ClaimTypes.Email,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddFrontendCors(builder.Configuration);

var antiforgerySameSite = builder.Configuration["Antiforgery:SameSite"];
var sameSite = antiforgerySameSite?.Equals("None", StringComparison.OrdinalIgnoreCase) == true
    ? SameSiteMode.None
    : SameSiteMode.Lax;

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = sameSite;
    options.Cookie.SecurePolicy = sameSite == SameSiteMode.None
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
});
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<IProductReadRepository, ProductReadRepository>();
builder.Services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
builder.Services.AddScoped<ICachedProductService, CachedProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPromoCodeService, PromoCodeService>();

builder.Services.Configure<SupabaseStorageOptions>(builder.Configuration.GetSection(SupabaseStorageOptions.SectionName));
builder.Services.AddSingleton<LocalImageStore>();
builder.Services.AddScoped<SupabaseImageStorage>();
builder.Services.AddScoped<IImageStorage, ConfiguredImageStorage>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<ProtectedOperationTransformer>();
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "AngularApp3");
});

app.UseHttpsRedirection();

app.UseCors(CorsOptions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
        var tokens = antiforgery.GetAndStoreTokens(context);
        if (!string.IsNullOrEmpty(tokens.RequestToken))
        {
            context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions
            {
                HttpOnly = false,
                Path = "/",
                IsEssential = true,
                SameSite = sameSite,
                Secure = sameSite == SameSiteMode.None
            });
        }
    }

    await next(context);
});

app.MapControllers();

app.MapFallbackToFile("/index.html");

await DatabaseMigrator.MigrateAsync(app.Services);
await IdentityDataSeeder.SeedRolesAsync(app.Services);
await IdentityDataSeeder.SeedAdminAsync(app.Services, app.Configuration);
await PromoCodeSeeder.SeedAsync(app.Services);

app.Run();
