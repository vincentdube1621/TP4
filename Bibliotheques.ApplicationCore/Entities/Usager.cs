using System.ComponentModel.DataAnnotations;

namespace Bibliotheques.ApplicationCore.Entities
{
    /// <summary>
    /// Représente un usager de la bibliothèque
    /// </summary>
    public class Usager
    {
        /// <summary>
        /// Identifiant unique de l'usager
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Prénom de l'usager
        /// </summary>
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [MaxLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'usager
        /// </summary>
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MaxLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Adresse courriel de l'usager
        /// </summary>
        [Required(ErrorMessage = "Le courriel est obligatoire")]
        [EmailAddress(ErrorMessage = "Adresse courriel invalide")]
        [Display(Name = "Courriel")]
        public string Courriel { get; set; } = string.Empty;

        /// <summary>
        /// Numéro de téléphone de l'usager (optionnel)
        /// </summary>
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }

        /// <summary>
        /// Nombre de défaillances de l'usager
        /// </summary>
        [Range(0, 3, ErrorMessage = "Le nombre de défaillances doit être entre 0 et 3")]
        [Display(Name = "Défaillances")]
        public int NombreDefaillances { get; set; } = 0;

        /// <summary>
        /// Collection des emprunts associés à cet usager
        /// </summary>
        public ICollection<Emprunt>? Emprunts { get; set; }

        /// <summary>
        /// Nom complet de l'usager
        /// </summary>
        public string NomComplet => $"{Prenom} {Nom}";

        /// <summary>
        /// Nombre d'emprunts actifs de cet usager
        /// </summary>
        public int NombreEmpruntsActifs => Emprunts?.Count(e => e.DateRetourReelle == null) ?? 0;

        /// <summary>
        /// Vérifie si l'usager peut emprunter (moins de 3 emprunts actifs et moins de 3 défaillances)
        /// </summary>
        public bool PeutEmprunter => NombreEmpruntsActifs < 3 && NombreDefaillances < 3;

        /// <summary>
        /// Vérifie si l'usager est bloqué (3 défaillances)
        /// </summary>
        public bool EstBloque => NombreDefaillances >= 3;
    }
}
