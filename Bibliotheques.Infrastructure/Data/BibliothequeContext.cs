using Microsoft.EntityFrameworkCore;
using Bibliotheques.ApplicationCore.Entities;

namespace Bibliotheques.Infrastructure.Data
{
    /// <summary>
    /// Contexte Entity Framework pour la bibliothèque
    /// </summary>
    public class BibliothequeContext : DbContext
    {
        public BibliothequeContext(DbContextOptions<BibliothequeContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Table des livres
        /// </summary>
        public DbSet<Livre> Livres { get; set; }

        /// <summary>
        /// Table des usagers
        /// </summary>
        public DbSet<Usager> Usagers { get; set; }

        /// <summary>
        /// Table des emprunts
        /// </summary>
        public DbSet<Emprunt> Emprunts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des relations
            modelBuilder.Entity<Emprunt>(entity =>
            {
                entity.HasOne(e => e.Livre)
                      .WithMany(l => l.Emprunts)
                      .HasForeignKey(e => e.LivreId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Usager)
                      .WithMany(u => u.Emprunts)
                      .HasForeignKey(e => e.UsagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuration des contraintes
            modelBuilder.Entity<Livre>(entity =>
            {
                entity.HasIndex(l => l.Isbn).IsUnique();
                entity.Property(l => l.Titre).IsRequired().HasMaxLength(150);
                entity.Property(l => l.Auteur).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Isbn).IsRequired().HasMaxLength(17);
            });

            modelBuilder.Entity<Usager>(entity =>
            {
                entity.Property(u => u.Prenom).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Nom).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Courriel).IsRequired().HasMaxLength(254);
                entity.Property(u => u.Telephone).HasMaxLength(20);
                entity.HasIndex(u => u.Courriel).IsUnique();
            });

            modelBuilder.Entity<Emprunt>(entity =>
            {
                entity.Property(e => e.DateEmprunt).IsRequired();
                entity.Property(e => e.DateRetourPrevue).IsRequired();
            });

            // Données de test
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Livres de test
            modelBuilder.Entity<Livre>().HasData(
                new Livre
                {
                    Id = 1,
                    Titre = "Clean Code",
                    Auteur = "Robert C. Martin",
                    Isbn = "9780132350884",
                    AnneePublication = 2008,
                    QuantiteTotale = 3,
                    QuantiteDisponible = 3
                },
                new Livre
                {
                    Id = 2,
                    Titre = "Design Patterns",
                    Auteur = "Gang of Four",
                    Isbn = "9780201633610",
                    AnneePublication = 1994,
                    QuantiteTotale = 2,
                    QuantiteDisponible = 2
                },
                new Livre
                {
                    Id = 3,
                    Titre = "Refactoring",
                    Auteur = "Martin Fowler",
                    Isbn = "9780134757599",
                    AnneePublication = 2018,
                    QuantiteTotale = 1,
                    QuantiteDisponible = 1
                }
            );

            // Usagers de test
            modelBuilder.Entity<Usager>().HasData(
                new Usager
                {
                    Id = 1,
                    Prenom = "Alice",
                    Nom = "Tremblay",
                    Courriel = "alice.tremblay@email.com",
                    Telephone = "418-555-0101",
                    NombreDefaillances = 0
                },
                new Usager
                {
                    Id = 2,
                    Prenom = "Bob",
                    Nom = "Gagnon",
                    Courriel = "bob.gagnon@email.com",
                    Telephone = "418-555-0102",
                    NombreDefaillances = 1
                },
                new Usager
                {
                    Id = 3,
                    Prenom = "Charlie",
                    Nom = "Dubois",
                    Courriel = "charlie.dubois@email.com",
                    Telephone = "418-555-0103",
                    NombreDefaillances = 3 // Bloqué
                }
            );
        }
    }
}
