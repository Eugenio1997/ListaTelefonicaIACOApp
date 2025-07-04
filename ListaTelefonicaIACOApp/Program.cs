using ListaTelefonicaIACOApp;
using ListaTelefonicaIACOApp.BackgroundServices;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Infrastructure.Seeding;
using ListaTelefonicaIACOApp.Models;
using ListaTelefonicaIACOApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ListaTelefonicaDbContext(configuration);
});

builder.Services.AddHostedService<LogLimpezaBackgroundService>();

builder.Services.AddScoped<IHashService, HashService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";     // Rota de login
        options.LogoutPath = "/Logout";   // Rota de logout
        options.Cookie.Name = "MeuCookieDeAutenticacao"; // Nome do cookie

    });

builder.Services.AddAuthorization();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

// Crie a conexão com Oracle
using (var connection = new OracleConnection(builder.Configuration.GetConnectionString("ListaTelefonicaIACOConnectionString")))
{
    connection.Open();
    DatabaseSeeder.Seed(connection);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contato}/{action=Index}/{id?}");

app.Run();
