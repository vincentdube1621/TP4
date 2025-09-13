using System.Linq.Expressions;

namespace Bibliotheques.ApplicationCore.Interfaces
{
    /// <summary>
    /// Interface générique pour le pattern Repository
    /// </summary>
    /// <typeparam name="T">Type d'entité</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Récupère toutes les entités
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Récupère une entité par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'entité</param>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Récupère des entités selon un critère
        /// </summary>
        /// <param name="predicate">Critère de recherche</param>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Récupère la première entité selon un critère
        /// </summary>
        /// <param name="predicate">Critère de recherche</param>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Ajoute une nouvelle entité
        /// </summary>
        /// <param name="entity">Entité à ajouter</param>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Met à jour une entité existante
        /// </summary>
        /// <param name="entity">Entité à mettre à jour</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Supprime une entité par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'entité</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Supprime une entité
        /// </summary>
        /// <param name="entity">Entité à supprimer</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Compte le nombre d'entités selon un critère
        /// </summary>
        /// <param name="predicate">Critère de comptage</param>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Vérifie si une entité existe selon un critère
        /// </summary>
        /// <param name="predicate">Critère de vérification</param>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
