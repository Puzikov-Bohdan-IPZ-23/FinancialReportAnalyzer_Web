using FinancialReportAnalyzer.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using FinancialReportAnalyzer.Web.Models; // Додаємо наші моделі

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//  РОБОТА З БАЗАМИ ДАНИХ 

// 1. Отримуємо ключ вибору провайдера
var provider = config["DatabaseProvider"];

// 2. Реєструємо ApplicationDbContext (для Identity)
// Він буде використовувати того ж провайдера, що і ReportDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (provider)
    {
        case "MsSql": // a. MS-SQL
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            break;
        case "Postgres": // b. Postgres
            options.UseNpgsql(config.GetConnectionString("PostgresConnection"));
            break;
        case "Sqlite": // c. Sqlite
            options.UseSqlite(config.GetConnectionString("SqliteConnection"));
            break;
        case "InMemory": // d. In-Memory
            options.UseInMemoryDatabase("InMemoryDb_Identity");
            break;
        default:
            throw new InvalidOperationException($"Database provider '{provider}' is not supported.");
    }
});

// 3. Реєструємо ReportDbContext (для бізнес-логіки)
// Він буде використовувати того ж провайдера, що і ApplicationDbContext
builder.Services.AddDbContext<ReportDbContext>(options =>
{
    switch (provider)
    {
        case "MsSql": // a. MS-SQL
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            break;
        case "Postgres": // b. Postgres
            options.UseNpgsql(config.GetConnectionString("PostgresConnection"));
            break;
        case "Sqlite": // c. Sqlite
            options.UseSqlite(config.GetConnectionString("SqliteConnection"));
            break;
        case "InMemory": // d. In-Memory
            options.UseInMemoryDatabase("InMemoryDb_Reports");
            break;
        default:
            throw new InvalidOperationException($"Database provider '{provider}' is not supported.");
    }
});




// 4. Налаштування Identity (використовує ApplicationDbContext)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>(); // <-- Важливо: вказуємо ApplicationDbContext

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();



builder.Services.ConfigureApplicationCookie(options =>
{
    // Встановлюємо, куди перенаправляти користувача
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

    // Ваш існуючий код для OnRedirectToAccessDenied
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.Redirect("/");
        return Task.CompletedTask;
    };
});

//  ДОДАЄМО Auth0 як зовнішнього провайдера
builder.Services.AddAuthentication() // 
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

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name"
        };

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
// КІНЕЦЬ АВТЕНТИФІКАЦІЇ 


var app = builder.Build();

// Налаштування конвеєра HTTP
if (app.Environment.IsDevelopment())
{
    // Включаємо PII для детальних помилок
    IdentityModelEventSource.ShowPII = true;
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Цей порядок є правильним і життєво важливим
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();