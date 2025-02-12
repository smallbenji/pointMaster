using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace pointMaster.Migrations
{
    /// <inheritdoc />
    public partial class Addingblockings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patruljer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patruljer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Poster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BlockPoint = table.Column<bool>(type: "boolean", nullable: false),
                    BlockTurnout = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatruljeMedlemmer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    PatruljeId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatruljeMedlemmer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatruljeMedlemmer_Patruljer_PatruljeId",
                        column: x => x.PatruljeId,
                        principalTable: "Patruljer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Turnout = table.Column<int>(type: "integer", nullable: false),
                    PatruljeId = table.Column<int>(type: "integer", nullable: true),
                    PosterId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Points_Patruljer_PatruljeId",
                        column: x => x.PatruljeId,
                        principalTable: "Patruljer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Points_Poster_PosterId",
                        column: x => x.PosterId,
                        principalTable: "Poster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatruljeMedlemmer_PatruljeId",
                table: "PatruljeMedlemmer",
                column: "PatruljeId");

            migrationBuilder.CreateIndex(
                name: "IX_Points_PatruljeId",
                table: "Points",
                column: "PatruljeId");

            migrationBuilder.CreateIndex(
                name: "IX_Points_PosterId",
                table: "Points",
                column: "PosterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatruljeMedlemmer");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Patruljer");

            migrationBuilder.DropTable(
                name: "Poster");
        }
    }
}
