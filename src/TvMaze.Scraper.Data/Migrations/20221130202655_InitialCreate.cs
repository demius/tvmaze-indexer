using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvMaze.Scraper.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "people",
                columns: table => new
                {
                    personid = table.Column<int>(name: "person_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    url = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    birthday = table.Column<DateTime>(type: "TEXT", nullable: true),
                    lastupdated = table.Column<uint>(name: "last_updated", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_people", x => x.personid);
                });

            migrationBuilder.CreateTable(
                name: "tv_shows",
                columns: table => new
                {
                    tvshowid = table.Column<int>(name: "tv_show_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    url = table.Column<string>(type: "TEXT", nullable: false),
                    lastupdated = table.Column<uint>(name: "last_updated", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tv_shows", x => x.tvshowid);
                });

            migrationBuilder.CreateTable(
                name: "person_tv_show",
                columns: table => new
                {
                    castmemberspersonid = table.Column<int>(name: "cast_members_person_id", type: "INTEGER", nullable: false),
                    tvshowstvshowid = table.Column<int>(name: "tv_shows_tv_show_id", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_tv_show", x => new { x.castmemberspersonid, x.tvshowstvshowid });
                    table.ForeignKey(
                        name: "fk_person_tv_show_people_cast_members_person_id",
                        column: x => x.castmemberspersonid,
                        principalTable: "people",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_tv_show_tv_shows_tv_shows_tv_show_id",
                        column: x => x.tvshowstvshowid,
                        principalTable: "tv_shows",
                        principalColumn: "tv_show_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_person_tv_show_tv_shows_tv_show_id",
                table: "person_tv_show",
                column: "tv_shows_tv_show_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "person_tv_show");

            migrationBuilder.DropTable(
                name: "people");

            migrationBuilder.DropTable(
                name: "tv_shows");
        }
    }
}
