using Microsoft.EntityFrameworkCore;

namespace BibliothèqueLIPAJOLI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Ajoutez vos DbSet ici
        // public DbSet<Livre> Livres { get; set; }
        // public DbSet<Auteur> Auteurs { get; set; }
        // public DbSet<Emprunt> Emprunts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuration des modèles ici
        }
    }
}

