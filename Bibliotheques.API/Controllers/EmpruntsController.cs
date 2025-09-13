using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Bibliotheques.ApplicationCore.Entities;
using Bibliotheques.ApplicationCore.Interfaces;

namespace Bibliotheques.API.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des emprunts
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmpruntsController : ControllerBase
    {
        private readonly IEmpruntService _empruntService;
        private readonly IConfiguration _configuration;

        public EmpruntsController(IEmpruntService empruntService, IConfiguration configuration)
        {
            _empruntService = empruntService;
            _configuration = configuration;
        }

        /// <summary>
        /// Récupère tous les emprunts avec leurs relations
        /// </summary>
        /// <returns>Liste de tous les emprunts</returns>
        /// <response code="200">Liste des emprunts récupérée avec succès</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Emprunt>), 200)]
        public async Task<ActionResult<IEnumerable<Emprunt>>> GetEmprunts()
        {
            var emprunts = await _empruntService.GetEmpruntsAvecRelationsAsync();
            return Ok(emprunts);
        }

        /// <summary>
        /// Récupère un emprunt par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'emprunt</param>
        /// <returns>L'emprunt demandé</returns>
        /// <response code="200">Emprunt trouvé</response>
        /// <response code="404">Emprunt non trouvé</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Emprunt), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Emprunt>> GetEmprunt(int id)
        {
            var emprunt = await _empruntService.GetEmpruntAvecRelationsAsync(id);
            
            if (emprunt == null)
                return NotFound($"Emprunt avec l'ID {id} non trouvé");

            return Ok(emprunt);
        }

        /// <summary>
        /// Crée un nouvel emprunt
        /// </summary>
        /// <param name="request">Données de l'emprunt à créer</param>
        /// <returns>L'emprunt créé</returns>
        /// <response code="201">Emprunt créé avec succès</response>
        /// <response code="400">Données invalides ou règles métier non respectées</response>
        [HttpPost]
        [ProducesResponseType(typeof(Emprunt), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Emprunt>> CreerEmprunt([FromBody] CreerEmpruntRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emprunt = await _empruntService.CreerEmpruntAsync(
                request.UsagerId, 
                request.LivreId, 
                request.DureeJours ?? 10);

            if (emprunt == null)
                return BadRequest("Impossible de créer l'emprunt. Vérifiez que l'usager peut emprunter et que le livre est disponible.");

            return CreatedAtAction(nameof(GetEmprunt), new { id = emprunt.Id }, emprunt);
        }

        /// <summary>
        /// Retourne un livre emprunté
        /// </summary>
        /// <param name="id">Identifiant de l'emprunt</param>
        /// <returns>Résultat de l'opération</returns>
        /// <response code="200">Livre retourné avec succès</response>
        /// <response code="400">Impossible de retourner le livre</response>
        /// <response code="404">Emprunt non trouvé</response>
        [HttpPut("{id}/retour")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> RetournerLivre(int id)
        {
            var emprunt = await _empruntService.GetEmpruntAvecRelationsAsync(id);
            if (emprunt == null)
                return NotFound($"Emprunt avec l'ID {id} non trouvé");

            var success = await _empruntService.RetournerLivreAsync(id);
            if (!success)
                return BadRequest("Impossible de retourner le livre");

            // Traiter les défaillances
            var joursLimite = _configuration.GetValue<int>("JoursLimiteRetour", 10);
            await _empruntService.TraiterDefaillanceAsync(emprunt, joursLimite);

            return Ok(new { message = "Livre retourné avec succès" });
        }

        /// <summary>
        /// Supprime un emprunt (seulement si pas encore retourné)
        /// </summary>
        /// <param name="id">Identifiant de l'emprunt</param>
        /// <returns>Résultat de l'opération</returns>
        /// <response code="200">Emprunt supprimé avec succès</response>
        /// <response code="400">Impossible de supprimer l'emprunt (déjà retourné)</response>
        /// <response code="404">Emprunt non trouvé</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> SupprimerEmprunt(int id)
        {
            var success = await _empruntService.SupprimerEmpruntAsync(id);
            if (!success)
                return BadRequest("Impossible de supprimer l'emprunt. Il a peut-être déjà été retourné.");

            return Ok(new { message = "Emprunt supprimé avec succès" });
        }

        /// <summary>
        /// Récupère les emprunts en retard
        /// </summary>
        /// <returns>Liste des emprunts en retard</returns>
        /// <response code="200">Liste des emprunts en retard</response>
        [HttpGet("en-retard")]
        [ProducesResponseType(typeof(IEnumerable<Emprunt>), 200)]
        public async Task<ActionResult<IEnumerable<Emprunt>>> GetEmpruntsEnRetard()
        {
            var emprunts = await _empruntService.GetEmpruntsEnRetardAsync();
            return Ok(emprunts);
        }

        /// <summary>
        /// Récupère les emprunts actifs
        /// </summary>
        /// <returns>Liste des emprunts actifs</returns>
        /// <response code="200">Liste des emprunts actifs</response>
        [HttpGet("actifs")]
        [ProducesResponseType(typeof(IEnumerable<Emprunt>), 200)]
        public async Task<ActionResult<IEnumerable<Emprunt>>> GetEmpruntsActifs()
        {
            var emprunts = await _empruntService.GetEmpruntsActifsAsync();
            return Ok(emprunts);
        }
    }

    /// <summary>
    /// Modèle de requête pour créer un emprunt
    /// </summary>
    public class CreerEmpruntRequest
    {
        /// <summary>
        /// Identifiant de l'usager
        /// </summary>
        [Required]
        public int UsagerId { get; set; }

        /// <summary>
        /// Identifiant du livre
        /// </summary>
        [Required]
        public int LivreId { get; set; }

        /// <summary>
        /// Durée de l'emprunt en jours (optionnel, défaut: 10)
        /// </summary>
        public int? DureeJours { get; set; }
    }
}
