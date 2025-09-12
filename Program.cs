using BibliothequeLIPAJOLI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BibliothequeLIPAJOLI.Interfaces;
using BibliothequeLIPAJOLI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("BibliothequeConnection");

builder.Services.AddDbContext<BibliothequeContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddSingleton<IParametresPret, ParametresPret>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BibliothequeContext>();
    db.Database.Migrate();
    DbInitializer.Seed(db);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public interface IParametresPret { int DureeJours { get; } }

public sealed class ParametresPret : IParametresPret
{
    public ParametresPret(IConfiguration cfg)
        => DureeJours = cfg.GetValue<int>("Pret:DureeJours", 10);
    public int DureeJours { get; }
}

public static class DbInitializer
{
    public static void Seed(BibliothequeContext db)
    {
        if (!db.Livres.Any())
        {
            db.Livres.AddRange(
                new Livre { Titre = "Clean Code", Auteur = "R. Martin", Isbn = "9780132350884", QuantiteTotale = 5, QuantiteDisponible = 5 },
                new Livre { Titre = "CLR via C#", Auteur = "J. Richter", Isbn = "9780735667457", QuantiteTotale = 3, QuantiteDisponible = 3 }
            );
        }

        if (!db.Usagers.Any())
        {
            db.Usagers.AddRange(
                new Usager { Prenom = "Alice", Nom = "Tremblay", Courriel = "alice@example.com" },
                new Usager { Prenom = "Bob", Nom = "Gagnon", Courriel = "bob@example.com" }
            );
        }

        db.SaveChanges();
    }
}
