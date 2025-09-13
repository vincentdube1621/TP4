using System.ComponentModel.DataAnnotations;

namespace BibliothequeLIPAJOLI.Models
{
    /// <summary>
    /// Attribut de validation personnalisé pour vérifier qu'une date n'est pas dans le futur
    /// </summary>
    public class NotFutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date <= DateTime.Now;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"La {name} ne peut pas être dans le futur.";
        }
    }

    /// <summary>
    /// Attribut de validation pour vérifier qu'une date de retour prévue est après la date de prêt
    /// </summary>
    public class DateRetourAfterDatePretAttribute : ValidationAttribute
    {
        private readonly string _datePretPropertyName;

        public DateRetourAfterDatePretAttribute(string datePretPropertyName)
        {
            _datePretPropertyName = datePretPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dateRetourPrevue = value as DateTime?;
            if (dateRetourPrevue == null) return ValidationResult.Success;

            var datePretProperty = validationContext.ObjectType.GetProperty(_datePretPropertyName);
            if (datePretProperty == null) return ValidationResult.Success;

            var datePret = datePretProperty.GetValue(validationContext.ObjectInstance) as DateTime?;
            if (datePret == null) return ValidationResult.Success;

            if (dateRetourPrevue <= datePret)
            {
                return new ValidationResult("La date de retour prévue doit être après la date de prêt.");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Attribut de validation pour vérifier qu'un ISBN est unique
    /// </summary>
    public class UniqueIsbnAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var isbn = value.ToString();
            if (string.IsNullOrWhiteSpace(isbn)) return ValidationResult.Success;

            // Cette validation sera complétée au niveau du contrôleur avec la base de données
            return ValidationResult.Success;
        }
    }
}
