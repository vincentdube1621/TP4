using Microsoft.EntityFrameworkCore;
using Bibliotheques.Infrastructure.Data;
using Bibliotheques.Infrastructure.Repositories;
using Bibliotheques.Infrastructure.Services;
using Bibliotheques.ApplicationCore.Interfaces;
using Bibliotheques.ApplicationCore.Entities;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=..\\lipajoli.db";

builder.Services.AddDbContext<BibliothequeContext>(options =>
    options.UseSqlite(connectionString));

// Enregistrement des services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEmpruntService, EmpruntService>();

// Configuration de l'API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "API Bibliothèque LIPAJOLI", 
        Version = "v1",
        Description = "API REST pour la gestion des emprunts de la bibliothèque LIPAJOLI"
    });
    
    // Inclure les commentaires XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configuration CORS pour permettre l'accès depuis le MVC
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configuration du pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Bibliothèque LIPAJOLI v1");
        c.RoutePrefix = string.Empty; // Swagger UI à la racine
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowMVC");
app.UseAuthorization();
app.MapControllers();

// Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BibliothequeContext>();
    context.Database.EnsureCreated();
}

app.Run();
