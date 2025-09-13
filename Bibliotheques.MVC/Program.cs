using Microsoft.EntityFrameworkCore;
using Bibliotheques.Infrastructure.Data;
using Bibliotheques.Infrastructure.Repositories;
using Bibliotheques.ApplicationCore.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données (même que l'API)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=..\\lipajoli.db";

builder.Services.AddDbContext<BibliothequeContext>(options =>
    options.UseSqlite(connectionString));

// Enregistrement des services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Configuration MVC
builder.Services.AddControllersWithViews();

// Configuration HttpClient pour consommer l'API
builder.Services.AddHttpClient("BibliothequeAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configuration du pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BibliothequeContext>();
    context.Database.EnsureCreated();
}

app.Run();
