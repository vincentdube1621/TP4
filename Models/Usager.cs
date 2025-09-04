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

        [EmailAddress(ErrorMessage = "Adresse email invalide")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string? Telephone { get; set; }

        // Navigation
        public ICollection<Pret>? Prets { get; set; }
    }
}
