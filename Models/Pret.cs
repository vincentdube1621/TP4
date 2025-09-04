using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliothequeLIPAJOLI.Models
{
    public class Pret
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DatePret { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateRetourPrevue { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateRetourReelle { get; set; }

        // Relations
        [ForeignKey("Livre")]
        public int LivreId { get; set; }
        public Livre Livre { get; set; }

        [ForeignKey("Usager")]
        public int UsagerId { get; set; }
        public Usager Usager { get; set; }
    }
}
