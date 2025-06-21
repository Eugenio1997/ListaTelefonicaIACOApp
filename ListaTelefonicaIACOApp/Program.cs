using ListaTelefonicaIACOApp;
using ListaTelefonicaIACOApp.Infrastructure;
using ListaTelefonicaIACOApp.Infrastructure.Seeding;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ListaTelefonicaDbContext(configuration);
});

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Autenticacao/Login";     // Rota de login
        options.LogoutPath = "/Autenticacao/Logout";   // Rota de logout
        options.AccessDeniedPath = "/Autenticacao/AcessoNegado";
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Sess�o de 30 minutos
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

// Crie a conex�o com Oracle
using (var connection = new OracleConnection(builder.Configuration.GetConnectionString("ListaTelefonicaIACOConnectionString")))
{
    //connection.Open();
    //DatabaseSeeder.Seed(connection);
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
