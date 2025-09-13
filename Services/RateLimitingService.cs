using System.Collections.Concurrent;

namespace BibliothequeLIPAJOLI.Services
{
    /// <summary>
    /// Service de limitation du taux de requêtes pour prévenir les abus
    /// </summary>
    public interface IRateLimitingService
    {
        bool EstAutorise(string cle, int limite, TimeSpan fenetre);
        void NettoyerCache();
    }

    /// <summary>
    /// Implémentation du service de limitation du taux
    /// </summary>
    public class RateLimitingService : IRateLimitingService
    {
        private readonly ConcurrentDictionary<string, List<DateTime>> _requetes = new();
        private readonly Timer _timerNettoyage;

        public RateLimitingService()
        {
            // Nettoyer le cache toutes les 5 minutes
            _timerNettoyage = new Timer(NettoyerCache, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public bool EstAutorise(string cle, int limite, TimeSpan fenetre)
        {
            var maintenant = DateTime.UtcNow;
            var fenetreDebut = maintenant - fenetre;

            // Obtenir ou créer la liste des requêtes pour cette clé
            var requetes = _requetes.GetOrAdd(cle, _ => new List<DateTime>());

            lock (requetes)
            {
                // Supprimer les requêtes anciennes
                requetes.RemoveAll(r => r < fenetreDebut);

                // Vérifier si la limite est atteinte
                if (requetes.Count >= limite)
                {
                    return false;
                }

                // Ajouter la nouvelle requête
                requetes.Add(maintenant);
                return true;
            }
        }

        public void NettoyerCache()
        {
            var maintenant = DateTime.UtcNow;
            var clesASupprimer = new List<string>();

            foreach (var kvp in _requetes)
            {
                lock (kvp.Value)
                {
                    // Supprimer les requêtes anciennes (plus de 1 heure)
                    kvp.Value.RemoveAll(r => r < maintenant.AddHours(-1));

                    // Si la liste est vide, marquer pour suppression
                    if (kvp.Value.Count == 0)
                    {
                        clesASupprimer.Add(kvp.Key);
                    }
                }
            }

            // Supprimer les clés vides
            foreach (var cle in clesASupprimer)
            {
                _requetes.TryRemove(cle, out _);
            }
        }
    }
}
