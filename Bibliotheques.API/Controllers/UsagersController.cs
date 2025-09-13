using Microsoft.AspNetCore.Mvc;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;

namespace Bibliotheques.API.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des usagers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsagersController : ControllerBase
    {
        private readonly IRepository<Usager> _usagerRepository;

        public UsagersController(IRepository<Usager> usagerRepository)
        {
            _usagerRepository = usagerRepository;
        }

        /// <summary>
        /// Récupère tous les usagers
        /// </summary>
        /// <returns>Liste de tous les usagers</returns>
        /// <response code="200">Liste des usagers récupérée avec succès</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Usager>), 200)]
        public async Task<ActionResult<IEnumerable<Usager>>> GetUsagers()
        {
            var usagers = await _usagerRepository.GetAllAsync();
            return Ok(usagers);
        }

        /// <summary>
        /// Récupère un usager par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'usager</param>
        /// <returns>L'usager demandé</returns>
        /// <response code="200">Usager trouvé</response>
        /// <response code="404">Usager non trouvé</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Usager), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Usager>> GetUsager(int id)
        {
            var usager = await _usagerRepository.GetByIdAsync(id);
            
            if (usager == null)
                return NotFound($"Usager avec l'ID {id} non trouvé");

            return Ok(usager);
        }
    }
}
