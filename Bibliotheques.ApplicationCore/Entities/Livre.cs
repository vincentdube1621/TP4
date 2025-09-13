using System.ComponentModel.DataAnnotations;

namespace Bibliotheques.ApplicationCore.Entities
{
    /// <summary>
    /// Représente un livre dans la bibliothèque
    /// </summary>
    public class Livre
    {
        /// <summary>
        /// Identifiant unique du livre
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Titre du livre
        /// </summary>
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [MaxLength(150, ErrorMessage = "Le titre ne peut pas dépasser 150 caractères")]
        [Display(Name = "Titre")]
        public string Titre { get; set; } = string.Empty;

        /// <summary>
        /// Auteur du livre
        /// </summary>
        [Required(ErrorMessage = "L'auteur est obligatoire")]
        [MaxLength(100, ErrorMessage = "Le nom de l'auteur ne peut pas dépasser 100 caractères")]
        [Display(Name = "Auteur")]
        public string Auteur { get; set; } = string.Empty;

        /// <summary>
        /// Année de publication du livre
        /// </summary>
        [Range(1500, 2100, ErrorMessage = "L'année doit être entre 1500 et 2100")]
        [Display(Name = "Année de publication")]
        public int AnneePublication { get; set; }

        /// <summary>
        /// Numéro ISBN du livre (ISBN-10 ou ISBN-13)
        /// </summary>
        [Required(ErrorMessage = "L'ISBN est obligatoire")]
        [Display(Name = "ISBN")]
        [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "ISBN invalide (ISBN-10 ou ISBN-13)")]
        public string Isbn { get; set; } = string.Empty;

        /// <summary>
        /// Quantité totale de copies du livre
        /// </summary>
        [Required(ErrorMessage = "La quantité totale est obligatoire")]
        [Range(0, 1000, ErrorMessage = "La quantité totale doit être entre 0 et 1000")]
        [Display(Name = "Quantité totale")]
        public int QuantiteTotale { get; set; }

        /// <summary>
        /// Quantité de copies disponibles pour emprunt
        /// </summary>
        [Required(ErrorMessage = "La quantité disponible est obligatoire")]
        [Range(0, 1000, ErrorMessage = "La quantité disponible doit être entre 0 et 1000")]
        [Display(Name = "Quantité disponible")]
        public int QuantiteDisponible { get; set; }

        /// <summary>
        /// Collection des prêts associés à ce livre
        /// </summary>
        public ICollection<Emprunt>? Emprunts { get; set; }

        /// <summary>
        /// Vérifie si le livre est disponible pour emprunt
        /// </summary>
        public bool EstDisponible => QuantiteDisponible > 0;

        /// <summary>
        /// Retourne le titre complet avec l'auteur
        /// </summary>
        public string TitreComplet => $"{Titre} - {Auteur}";
    }
}
