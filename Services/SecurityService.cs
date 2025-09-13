using System.Text.RegularExpressions;

namespace BibliothequeLIPAJOLI.Services
{
    /// <summary>
    /// Service de sécurité pour la validation et la protection des données
    /// </summary>
    public interface ISecurityService
    {
        bool ValiderEmail(string email);
        bool ValiderTelephone(string telephone);
        bool ValiderIsbn(string isbn);
        string SanitizerHtml(string input);
        bool ValiderLongueurTexte(string texte, int longueurMax);
        bool ValiderCaracteresAutorises(string texte, string pattern);
    }

    /// <summary>
    /// Implémentation du service de sécurité
    /// </summary>
    public class SecurityService : ISecurityService
    {
        private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        private static readonly Regex TelephoneRegex = new(@"^[\+]?[1-9][\d]{0,15}$", RegexOptions.Compiled);
        private static readonly Regex IsbnRegex = new(@"^(97(8|9))?\d{9}(\d|X)$", RegexOptions.Compiled);
        private static readonly Regex HtmlRegex = new(@"<[^>]*>", RegexOptions.Compiled);

        public bool ValiderEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email) && email.Length <= 254;
        }

        public bool ValiderTelephone(string telephone)
        {
            if (string.IsNullOrWhiteSpace(telephone))
                return true; // Le téléphone est optionnel

            // Nettoyer le numéro (enlever espaces, tirets, parenthèses)
            var numeroNettoye = Regex.Replace(telephone, @"[\s\-\(\)]", "");
            
            return TelephoneRegex.IsMatch(numeroNettoye) && numeroNettoye.Length >= 10 && numeroNettoye.Length <= 15;
        }

        public bool ValiderIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return false;

            return IsbnRegex.IsMatch(isbn);
        }

        public string SanitizerHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Supprimer les balises HTML
            var sansHtml = HtmlRegex.Replace(input, "");
            
            // Encoder les caractères spéciaux
            return sansHtml.Replace("&", "&amp;")
                          .Replace("<", "&lt;")
                          .Replace(">", "&gt;")
                          .Replace("\"", "&quot;")
                          .Replace("'", "&#x27;");
        }

        public bool ValiderLongueurTexte(string texte, int longueurMax)
        {
            if (texte == null)
                return true;

            return texte.Length <= longueurMax;
        }

        public bool ValiderCaracteresAutorises(string texte, string pattern)
        {
            if (string.IsNullOrWhiteSpace(texte))
                return true;

            var regex = new Regex(pattern, RegexOptions.Compiled);
            return regex.IsMatch(texte);
        }
    }
}
