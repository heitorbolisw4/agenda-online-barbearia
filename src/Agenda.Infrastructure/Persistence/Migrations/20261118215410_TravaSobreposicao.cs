using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agenda.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Trava no banco: o mesmo profissional não pode ter dois agendamentos
    /// ativos com horários que se sobrepõem. Se duas reservas chegarem juntas,
    /// o Postgres aceita uma e recusa a outra (erro 23P01).
    /// O EF Core não sabe criar EXCLUDE, então vai em SQL puro.
    /// </summary>
    public partial class TravaSobreposicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // btree_gist deixa o índice GiST comparar uuid com "=".
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            // [) = início incluso, fim excluso: 09:00-09:30 e 09:30-10:00 não colidem.
            migrationBuilder.Sql("""
                ALTER TABLE "Agendamentos"
                ADD CONSTRAINT ex_agendamentos_sem_sobreposicao
                EXCLUDE USING gist (
                    "ProfissionalId" WITH =,
                    tstzrange("Inicio", "Fim", '[)') WITH &&
                )
                WHERE ("Status" <> 'Cancelado');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""ALTER TABLE "Agendamentos" DROP CONSTRAINT IF EXISTS ex_agendamentos_sem_sobreposicao;""");
        }
    }
}
