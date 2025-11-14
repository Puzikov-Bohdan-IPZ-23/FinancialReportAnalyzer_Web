using FinancialReportAnalyzer.Web.Data;
using FinancialReportAnalyzer.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
// --- ДОДАНО ДЛЯ ЛАБ. 5 ---
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
// --- КІНЕЦЬ БЛОКУ ---


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//  РОБОТА З БАЗАМИ ДАНИХ 
var provider = config["DatabaseProvider"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (provider)
    {
        case "MsSql": options.UseSqlServer(config.GetConnectionString("DefaultConnection")); break;
        case "Postgres": options.UseNpgsql(config.GetConnectionString("PostgresConnection")); break;
        case "Sqlite": options.UseSqlite(config.GetConnectionString("SqliteConnection")); break;
        case "InMemory": options.UseInMemoryDatabase("InMemoryDb_Identity"); break;
        default: throw new InvalidOperationException($"Database provider '{provider}' is not supported.");
    }
});

builder.Services.AddDbContext<ReportDbContext>(options =>
{
    switch (provider)
    {
        case "MsSql": options.UseSqlServer(config.GetConnectionString("DefaultConnection")); break;
        case "Postgres": options.UseNpgsql(config.GetConnectionString("PostgresConnection")); break;
        case "Sqlite": options.UseSqlite(config.GetConnectionString("SqliteConnection")); break;
        case "InMemory": options.UseInMemoryDatabase("InMemoryDb_Reports"); break;
        default: throw new InvalidOperationException($"Database provider '{provider}' is not supported.");
    }
});

// Налаштування Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- ОНОВЛЕНО ДЛЯ ЛАБ. 5 ---
// Додаємо .AddNewtonsoftJson() для уникнення циклічних посилань в API
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );
// --- КІНЕЦЬ БЛОКУ ---


builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.Redirect("/");
        return Task.CompletedTask;
    };
});

// Auth0
builder.Services.AddAuthentication()
    .AddOpenIdConnect("Auth0", "Auth0", options =>
    {
        options.Authority = $"https://{config["Auth0:Domain"]}";
        options.ClientId = config["Auth0:ClientId"];
        options.ClientSecret = config["Auth0:ClientSecret"];
        options.ResponseType = "code";
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.CallbackPath = new PathString("/callback-oidc");
        options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" };
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProviderForSignOut = (context) =>
            {
                var logoutUri = $"https://{config["Auth0:Domain"]}/v2/logout?client_id={config["Auth0:ClientId"]}";
                var postLogoutUri = context.Properties.RedirectUri;
                if (!string.IsNullOrEmpty(postLogoutUri))
                {
                    if (postLogoutUri.StartsWith("/"))
                    {
                        var request = context.Request;
                        postLogoutUri = request.Scheme + "://" + request.Host + postLogoutUri;
                    }
                    logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                }
                context.Response.Redirect(logoutUri);
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    });

// --- БЛОК ДЛЯ ЛАБОРАТОРНОЇ 5 (ЕТАП 2 і 4) ---

// 1. Додаємо підтримку версіонування API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // Версія за замовчуванням v1.0
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Повідомляти про підтримувані версії у заголовках
    // Читати версію з URL (напр. /api/v1/...)
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// 2. Додаємо інтеграцію версіонування зі Swagger
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Формат імені групи у Swagger (напр. v1)
    options.SubstituteApiVersionInUrl = true;
});

// 3. Налаштовуємо генератор Swagger (для документації API)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Financial API", Version = "v1.0" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "Financial API", Version = "v2.0" });
});
// --- КІНЕЦЬ БЛОКУ ЛАБ. 5 ---


var app = builder.Build();

// Налаштування конвеєра HTTP
if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
    app.UseMigrationsEndPoint();

    // --- БЛОК ДЛЯ ЛАБОРАТОРНОЇ 5 (ЕТАП 2 і 4) ---
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Створюємо окремий ендпоінт для кожної версії у Swagger UI
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
        options.RoutePrefix = "swagger"; // Документація буде доступна за адресою /swagger
    });
    // --- КІНЕЦЬ БЛОКУ ЛАБ. 5 ---
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// --- ДОДАНО ДЛЯ ЛАБ. 5 ---
// Цей рядок вмикає обробку [ApiController] атрибутів
app.MapControllers();
// --- КІНЕЦЬ БЛОКУ ---

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();