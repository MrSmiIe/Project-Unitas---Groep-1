using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_Unitas.Data;
using Project_Unitas.Models;
using Project_Unitas.Services;

var builder = WebApplication.CreateBuilder(args);

// Don't tell visitors which web server software runs the site ("Server: Kestrel").
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// ---------------------------------------------------------------------------
// Database (MySQL). The connection string is NOT stored in git:
//  - on your own computer: dotnet user-secrets (see db/README.md)
//  - on the web server:    environment variable ConnectionStrings__Unitas
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Unitas");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'Unitas' ontbreekt. Zie db/README.md voor het instellen van de database.");
}

builder.Services.AddDbContext<UnitasDbContext>(options => options.UseMySQL(connectionString));

// ---------------------------------------------------------------------------
// Customer accounts (ASP.NET Core Identity) with the security policy from the brief:
// strong passwords, lockout after failed attempts, MFA via an authenticator app.
// ---------------------------------------------------------------------------
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false; // TODO: e-mail confirmation once the site can send e-mail
    })
    .AddEntityFrameworkStores<UnitasDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<DutchIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".Unitas.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTPS-only once the site runs on HTTPS
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.AddRazorPages(options =>
{
    // Everything under /Account requires a login, except the pages to get logged in.
    options.Conventions.AuthorizeFolder("/Account");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/LoginWith2fa");
    options.Conventions.AllowAnonymousToPage("/Account/LoginWithRecoveryCode");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
});

// Shopping cart: kept in a server-side session, the browser only gets a session cookie.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".Unitas.Session";
    options.Cookie.HttpOnly = true;              // not readable from JavaScript
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// On the web server the site runs behind Apache (reverse proxy on the same machine).
// Trust its X-Forwarded-* headers so logs show the visitor's real IP address.
// Only proxies on localhost are trusted (the default), so visitors can't fake these headers.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// Every cookie (login, session, form protection, messages) gets the "Secure" flag on the
// server, so browsers never send them over plain HTTP. Locally (Development) the site
// runs on http://localhost, so there it follows the request.
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

builder.Services.AddScoped<MachineCatalog>();
builder.Services.AddScoped<CartService>();

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseCookiePolicy();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
