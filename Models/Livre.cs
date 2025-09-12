using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliothequeLIPAJOLI.Models
{
    public class Livre
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [MaxLength(150)]
        public string Titre { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire")]
        [MaxLength(100)]
        public string Auteur { get; set; }

        [Range(1500, 2100, ErrorMessage = "L'année doit être entre 1500 et 2100")]
        public int AnneePublication { get; set; }

        [Required]
        [Display(Name = "ISBN")]
        [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "ISBN invalide (ISBN-10 ou ISBN-13)")]
        public string Isbn { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Quantité totale doit être >= 0")]
        [Display(Name = "Quantité totale")]
        public int QuantiteTotale { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Quantité disponible doit être >= 0")]
        [Display(Name = "Quantité disponible")]
        public int QuantiteDisponible { get; set; }

        // Navigation
        public ICollection<Pret>? Prets { get; set; }
    }
}
