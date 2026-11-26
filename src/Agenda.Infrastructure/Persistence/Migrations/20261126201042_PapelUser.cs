using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agenda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PapelUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Papel",
                table: "Profissionais",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Barbeiro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Papel",
                table: "Profissionais");
        }
    }
}
