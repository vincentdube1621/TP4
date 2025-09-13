using Microsoft.AspNetCore.Mvc;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;

namespace Bibliotheques.API.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des livres
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LivresController : ControllerBase
    {
        private readonly IRepository<Livre> _livreRepository;

        public LivresController(IRepository<Livre> livreRepository)
        {
            _livreRepository = livreRepository;
        }

        /// <summary>
        /// Récupère tous les livres
        /// </summary>
        /// <returns>Liste de tous les livres</returns>
        /// <response code="200">Liste des livres récupérée avec succès</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Livre>), 200)]
        public async Task<ActionResult<IEnumerable<Livre>>> GetLivres()
        {
            var livres = await _livreRepository.GetAllAsync();
            return Ok(livres);
        }

        /// <summary>
        /// Récupère un livre par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du livre</param>
        /// <returns>Le livre demandé</returns>
        /// <response code="200">Livre trouvé</response>
        /// <response code="404">Livre non trouvé</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Livre), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Livre>> GetLivre(int id)
        {
            var livre = await _livreRepository.GetByIdAsync(id);
            
            if (livre == null)
                return NotFound($"Livre avec l'ID {id} non trouvé");

            return Ok(livre);
        }
    }
}
