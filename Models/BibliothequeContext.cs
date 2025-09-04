using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BibliothequeLIPAJOLI.Models
{
    public class BibliothequeContext : DbContext
    {
        public BibliothequeContext(DbContextOptions<BibliothequeContext> options)
            : base(options)
        {
        }

        // Tables de la BD
        public DbSet<Livre> Livres { get; set; }
        public DbSet<Usager> Usagers { get; set; }
        public DbSet<Pret> Prets { get; set; }
    }
}
