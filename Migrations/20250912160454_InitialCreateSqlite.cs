using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliothèqueLIPAJOLI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Livres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Auteur = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AnneePublication = table.Column<int>(type: "INTEGER", nullable: false),
                    Isbn = table.Column<string>(type: "TEXT", nullable: false),
                    QuantiteTotale = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantiteDisponible = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usagers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Courriel = table.Column<string>(type: "TEXT", nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DatePret = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateRetourPrevue = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateRetourReelle = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LivreId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsagerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prets_Livres_LivreId",
                        column: x => x.LivreId,
                        principalTable: "Livres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prets_Usagers_UsagerId",
                        column: x => x.UsagerId,
                        principalTable: "Usagers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prets_LivreId",
                table: "Prets",
                column: "LivreId");

            migrationBuilder.CreateIndex(
                name: "IX_Prets_UsagerId",
                table: "Prets",
                column: "UsagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prets");

            migrationBuilder.DropTable(
                name: "Livres");

            migrationBuilder.DropTable(
                name: "Usagers");
        }
    }
}
