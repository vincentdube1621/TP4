using BibliothequeLIPAJOLI.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace BibliothequeLIPAJOLI.Tests
{
    /// <summary>
    /// Tests unitaires pour les modèles
    /// </summary>
    public class ModelTests
    {
        [Fact]
        public void Livre_EstDisponible_QuantiteDisponibleSuperieureAZero_RetourneTrue()
        {
            // Arrange
            var livre = new Livre
            {
                QuantiteDisponible = 1
            };

            // Act & Assert
            Assert.True(livre.EstDisponible);
        }

        [Fact]
        public void Livre_EstDisponible_QuantiteDisponibleEgaleAZero_RetourneFalse()
        {
            // Arrange
            var livre = new Livre
            {
                QuantiteDisponible = 0
            };

            // Act & Assert
            Assert.False(livre.EstDisponible);
        }

        [Fact]
        public void Livre_TitreComplet_RetourneTitreEtAuteur()
        {
            // Arrange
            var livre = new Livre
            {
                Titre = "Test Book",
                Auteur = "Test Author"
            };

            // Act & Assert
            Assert.Equal("Test Book - Test Author", livre.TitreComplet);
        }

        [Fact]
        public void Usager_NomComplet_RetournePrenomEtNom()
        {
            // Arrange
            var usager = new Usager
            {
                Prenom = "John",
                Nom = "Doe"
            };

            // Act & Assert
            Assert.Equal("John Doe", usager.NomComplet);
        }

        [Fact]
        public void Usager_PeutEmprunter_PasDePretsActifs_RetourneTrue()
        {
            // Arrange
            var usager = new Usager
            {
                Prets = new List<Pret>()
            };

            // Act & Assert
            Assert.True(usager.PeutEmprunter);
        }

        [Fact]
        public void Usager_PeutEmprunter_TroisPretsActifs_RetourneFalse()
        {
            // Arrange
            var usager = new Usager
            {
                Prets = new List<Pret>
                {
                    new Pret { DateRetourReelle = null },
                    new Pret { DateRetourReelle = null },
                    new Pret { DateRetourReelle = null }
                }
            };

            // Act & Assert
            Assert.False(usager.PeutEmprunter);
        }

        [Fact]
        public void Pret_EstActif_DateRetourReelleNull_RetourneTrue()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = null
            };

            // Act & Assert
            Assert.True(pret.EstActif);
        }

        [Fact]
        public void Pret_EstActif_DateRetourReelleDefinie_RetourneFalse()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = DateTime.Now
            };

            // Act & Assert
            Assert.False(pret.EstActif);
        }

        [Fact]
        public void Pret_EstEnRetard_DateRetourPrevueDansLePasse_RetourneTrue()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = null,
                DateRetourPrevue = DateTime.Now.AddDays(-1)
            };

            // Act & Assert
            Assert.True(pret.EstEnRetard);
        }

        [Fact]
        public void Pret_EstEnRetard_DateRetourPrevueDansLeFutur_RetourneFalse()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = null,
                DateRetourPrevue = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.False(pret.EstEnRetard);
        }

        [Fact]
        public void Pret_JoursDeRetard_PretEnRetard_RetourneNombreJoursCorrect()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = null,
                DateRetourPrevue = DateTime.Now.AddDays(-5)
            };

            // Act & Assert
            Assert.Equal(5, pret.JoursDeRetard);
        }

        [Fact]
        public void Pret_Statut_PretActif_RetourneActif()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = null,
                DateRetourPrevue = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.Equal("Actif", pret.Statut);
        }

        [Fact]
        public void Pret_Statut_PretEnRetard_RetourneEnRetard()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = null,
                DateRetourPrevue = DateTime.Now.AddDays(-3)
            };

            // Act & Assert
            Assert.Equal("En retard (3 jours)", pret.Statut);
        }

        [Fact]
        public void Pret_Statut_PretRetourne_RetourneRetourne()
        {
            // Arrange
            var pret = new Pret
            {
                DateRetourReelle = DateTime.Now
            };

            // Act & Assert
            Assert.Equal("Retourné", pret.Statut);
        }

        [Theory]
        [InlineData("9780132350884", true)]  // ISBN-13 valide
        [InlineData("0132350882", true)]     // ISBN-10 valide
        [InlineData("123456789X", true)]     // ISBN-10 avec X
        [InlineData("123456789", false)]     // ISBN-10 invalide (trop court)
        [InlineData("12345678901", false)]   // ISBN-10 invalide (trop long)
        [InlineData("978013235088", false)]  // ISBN-13 invalide (trop court)
        public void Livre_Isbn_ValidationRegex(string isbn, bool expectedValid)
        {
            // Arrange
            var livre = new Livre { Isbn = isbn };
            var context = new ValidationContext(livre);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(livre, context, results, true);

            // Assert
            if (expectedValid)
            {
                Assert.True(isValid);
            }
            else
            {
                Assert.False(isValid);
                Assert.Contains(results, r => r.ErrorMessage!.Contains("ISBN invalide"));
            }
        }
    }
}
