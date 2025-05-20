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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Colaborador}/{action=Index}/{id?}");

app.Run();
