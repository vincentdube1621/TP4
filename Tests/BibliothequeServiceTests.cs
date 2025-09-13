using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using BibliothequeLIPAJOLI.Models;
using BibliothequeLIPAJOLI.Services;
using Xunit;

namespace BibliothequeLIPAJOLI.Tests
{
    /// <summary>
    /// Tests unitaires pour le service BibliothequeService
    /// </summary>
    public class BibliothequeServiceTests : IDisposable
    {
        private readonly BibliothequeContext _context;
        private readonly Mock<ILoggingService> _mockLoggingService;
        private readonly Mock<IParametresPret> _mockParametresPret;
        private readonly BibliothequeService _service;

        public BibliothequeServiceTests()
        {
            // Configuration de la base de données en mémoire pour les tests
            var options = new DbContextOptionsBuilder<BibliothequeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BibliothequeContext(options);
            _mockLoggingService = new Mock<ILoggingService>();
            _mockParametresPret = new Mock<IParametresPret>();
            _mockParametresPret.Setup(p => p.DureeJours).Returns(10);

            _service = new BibliothequeService(_context, _mockLoggingService.Object, _mockParametresPret.Object);

            // Données de test
            SeedTestData();
        }

        private void SeedTestData()
        {
            var livre = new Livre
            {
                Id = 1,
                Titre = "Test Book",
                Auteur = "Test Author",
                Isbn = "1234567890",
                QuantiteTotale = 2,
                QuantiteDisponible = 2
            };

            var usager = new Usager
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                Courriel = "john.doe@test.com"
            };

            _context.Livres.Add(livre);
            _context.Usagers.Add(usager);
            _context.SaveChanges();
        }

        [Fact]
        public async Task PeutEmprunterLivreAsync_UsagerEtLivreValides_RetourneTrue()
        {
            // Act
            var result = await _service.PeutEmprunterLivreAsync(1, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PeutEmprunterLivreAsync_UsagerInexistant_RetourneFalse()
        {
            // Act
            var result = await _service.PeutEmprunterLivreAsync(999, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PeutEmprunterLivreAsync_LivreInexistant_RetourneFalse()
        {
            // Act
            var result = await _service.PeutEmprunterLivreAsync(1, 999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PeutEmprunterLivreAsync_LivreNonDisponible_RetourneFalse()
        {
            // Arrange
            var livre = await _context.Livres.FindAsync(1);
            livre!.QuantiteDisponible = 0;
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.PeutEmprunterLivreAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmprunterLivreAsync_EmpruntValide_RetourneTrue()
        {
            // Act
            var result = await _service.EmprunterLivreAsync(1, 1);

            // Assert
            Assert.True(result);
            
            // Vérifier que le prêt a été créé
            var pret = await _context.Prets.FirstOrDefaultAsync(p => p.UsagerId == 1 && p.LivreId == 1);
            Assert.NotNull(pret);
            Assert.True(pret.EstActif);

            // Vérifier que la quantité disponible a été décrémentée
            var livre = await _context.Livres.FindAsync(1);
            Assert.Equal(1, livre!.QuantiteDisponible);
        }

        [Fact]
        public async Task RetournerLivreAsync_PretValide_RetourneTrue()
        {
            // Arrange
            await _service.EmprunterLivreAsync(1, 1);
            var pret = await _context.Prets.FirstOrDefaultAsync(p => p.UsagerId == 1 && p.LivreId == 1);

            // Act
            var result = await _service.RetournerLivreAsync(pret!.Id);

            // Assert
            Assert.True(result);
            Assert.NotNull(pret.DateRetourReelle);
            Assert.False(pret.EstActif);

            // Vérifier que la quantité disponible a été incrémentée
            var livre = await _context.Livres.FindAsync(1);
            Assert.Equal(2, livre!.QuantiteDisponible);
        }

        [Fact]
        public async Task GetPretsEnRetardAsync_RetournePretsEnRetard()
        {
            // Arrange
            await _service.EmprunterLivreAsync(1, 1);
            var pret = await _context.Prets.FirstOrDefaultAsync(p => p.UsagerId == 1 && p.LivreId == 1);
            pret!.DateRetourPrevue = DateTime.Now.AddDays(-5); // 5 jours en retard
            await _context.SaveChangesAsync();

            // Act
            var pretsEnRetard = await _service.GetPretsEnRetardAsync();

            // Assert
            Assert.Single(pretsEnRetard);
            Assert.True(pretsEnRetard.First().EstEnRetard);
        }

        [Fact]
        public async Task RechercherLivresAsync_TermeValide_RetourneLivresCorrespondants()
        {
            // Act
            var result = await _service.RechercherLivresAsync("Test");

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Book", result.First().Titre);
        }

        [Fact]
        public async Task RechercherLivresAsync_TermeVide_RetourneTousLesLivres()
        {
            // Act
            var result = await _service.RechercherLivresAsync("");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetStatistiquesAsync_RetourneStatistiquesCorrectes()
        {
            // Act
            var stats = await _service.GetStatistiquesAsync();

            // Assert
            Assert.Equal(1, stats.TotalLivres);
            Assert.Equal(1, stats.TotalUsagers);
            Assert.Equal(0, stats.PretsActifs);
            Assert.Equal(0, stats.PretsEnRetard);
            Assert.Equal(2, stats.LivresDisponibles);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
