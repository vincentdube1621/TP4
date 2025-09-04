using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliothequeLIPAJOLI.Models
{
    public class Livre
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [MaxLength(100)]
        public string Titre { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire")]
        [MaxLength(50)]
        public string Auteur { get; set; }

        [Range(1500, 2100, ErrorMessage = "L'année doit être entre 1500 et 2100")]
        public int AnneePublication { get; set; }

        [Display(Name = "Disponible ?")]
        public bool EstDisponible { get; set; } = true;

        // Navigation
        public ICollection<Pret>? Prets { get; set; }
    }
}
