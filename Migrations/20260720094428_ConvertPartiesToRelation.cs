using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Politicore.Migrations
{
    /// <inheritdoc />
    public partial class ConvertPartiesToRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parties",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "PartyUser",
                columns: table => new
                {
                    PartiesId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyUser", x => new { x.PartiesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_PartyUser_Parties_PartiesId",
                        column: x => x.PartiesId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartyUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartyUser_UsersId",
                table: "PartyUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartyUser");

            migrationBuilder.AddColumn<string>(
                name: "Parties",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
