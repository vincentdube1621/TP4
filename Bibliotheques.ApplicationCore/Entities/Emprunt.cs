using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bibliotheques.ApplicationCore.Entities
{
    /// <summary>
    /// Représente un emprunt de livre dans la bibliothèque
    /// </summary>
    public class Emprunt
    {
        /// <summary>
        /// Identifiant unique de l'emprunt
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date à laquelle le livre a été emprunté
        /// </summary>
        [Required(ErrorMessage = "La date d'emprunt est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date d'emprunt")]
        public DateTime DateEmprunt { get; set; } = DateTime.Now;

        /// <summary>
        /// Date prévue pour le retour du livre
        /// </summary>
        [Required(ErrorMessage = "La date de retour prévue est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de retour prévue")]
        public DateTime DateRetourPrevue { get; set; }

        /// <summary>
        /// Date réelle de retour du livre (null si pas encore retourné)
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Date de retour réelle")]
        public DateTime? DateRetourReelle { get; set; }

        /// <summary>
        /// Indique si l'emprunt a causé une défaillance
        /// </summary>
        [Display(Name = "Défaillance")]
        public bool EstDefaillance { get; set; } = false;

        /// <summary>
        /// Identifiant du livre emprunté
        /// </summary>
        [ForeignKey("Livre")]
        [Display(Name = "Livre")]
        public int LivreId { get; set; }

        /// <summary>
        /// Livre emprunté
        /// </summary>
        public Livre Livre { get; set; } = null!;

        /// <summary>
        /// Identifiant de l'usager qui emprunte
        /// </summary>
        [ForeignKey("Usager")]
        [Display(Name = "Usager")]
        public int UsagerId { get; set; }

        /// <summary>
        /// Usager qui emprunte le livre
        /// </summary>
        public Usager Usager { get; set; } = null!;

        /// <summary>
        /// Vérifie si l'emprunt est actif (pas encore retourné)
        /// </summary>
        public bool EstActif => DateRetourReelle == null;

        /// <summary>
        /// Vérifie si l'emprunt est en retard
        /// </summary>
        public bool EstEnRetard => EstActif && DateRetourPrevue < DateTime.Now;

        /// <summary>
        /// Nombre de jours de retard (0 si pas en retard)
        /// </summary>
        public int JoursDeRetard => EstEnRetard ? (DateTime.Now - DateRetourPrevue).Days : 0;

        /// <summary>
        /// Durée de l'emprunt en jours
        /// </summary>
        public int DureeEmprunt => (DateRetourReelle ?? DateTime.Now - DateEmprunt).Days;

        /// <summary>
        /// Statut de l'emprunt sous forme de texte
        /// </summary>
        public string Statut
        {
            get
            {
                if (DateRetourReelle.HasValue)
                    return EstDefaillance ? "Retourné (Défaillance)" : "Retourné";
                if (EstEnRetard)
                    return $"En retard ({JoursDeRetard} jours)";
                return "Actif";
            }
        }
    }
}
