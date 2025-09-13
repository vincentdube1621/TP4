using Microsoft.Extensions.Logging;

namespace BibliothequeLIPAJOLI.Services
{
    /// <summary>
    /// Service de logging personnalisé pour la bibliothèque
    /// </summary>
    public interface ILoggingService
    {
        void LogLivreCree(int livreId, string titre);
        void LogLivreModifie(int livreId, string titre);
        void LogLivreSupprime(int livreId, string titre);
        void LogUsagerCree(int usagerId, string nom);
        void LogUsagerModifie(int usagerId, string nom);
        void LogUsagerSupprime(int usagerId, string nom);
        void LogPretCree(int pretId, string livreTitre, string usagerNom);
        void LogPretRetourne(int pretId, string livreTitre, string usagerNom);
        void LogErreur(string message, Exception? exception = null);
        void LogAvertissement(string message);
        void LogInformation(string message);
    }

    /// <summary>
    /// Implémentation du service de logging
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogLivreCree(int livreId, string titre)
        {
            _logger.LogInformation("Livre créé - ID: {LivreId}, Titre: {Titre}", livreId, titre);
        }

        public void LogLivreModifie(int livreId, string titre)
        {
            _logger.LogInformation("Livre modifié - ID: {LivreId}, Titre: {Titre}", livreId, titre);
        }

        public void LogLivreSupprime(int livreId, string titre)
        {
            _logger.LogWarning("Livre supprimé - ID: {LivreId}, Titre: {Titre}", livreId, titre);
        }

        public void LogUsagerCree(int usagerId, string nom)
        {
            _logger.LogInformation("Usager créé - ID: {UsagerId}, Nom: {Nom}", usagerId, nom);
        }

        public void LogUsagerModifie(int usagerId, string nom)
        {
            _logger.LogInformation("Usager modifié - ID: {UsagerId}, Nom: {Nom}", usagerId, nom);
        }

        public void LogUsagerSupprime(int usagerId, string nom)
        {
            _logger.LogWarning("Usager supprimé - ID: {UsagerId}, Nom: {Nom}", usagerId, nom);
        }

        public void LogPretCree(int pretId, string livreTitre, string usagerNom)
        {
            _logger.LogInformation("Prêt créé - ID: {PretId}, Livre: {LivreTitre}, Usager: {UsagerNom}", 
                pretId, livreTitre, usagerNom);
        }

        public void LogPretRetourne(int pretId, string livreTitre, string usagerNom)
        {
            _logger.LogInformation("Prêt retourné - ID: {PretId}, Livre: {LivreTitre}, Usager: {UsagerNom}", 
                pretId, livreTitre, usagerNom);
        }

        public void LogErreur(string message, Exception? exception = null)
        {
            _logger.LogError(exception, "Erreur: {Message}", message);
        }

        public void LogAvertissement(string message)
        {
            _logger.LogWarning("Avertissement: {Message}", message);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation("Information: {Message}", message);
        }
    }
}
