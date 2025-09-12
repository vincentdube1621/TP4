using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliothequeLIPAJOLI.Models
{
    public class Usager
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [MaxLength(50)]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MaxLength(50)]
        public string Nom { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Adresse courriel invalide")]
        [Display(Name = "Courriel")]
        public string Courriel { get; set; }

        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }

        // Navigation
        public ICollection<Pret>? Prets { get; set; }
    }
}
