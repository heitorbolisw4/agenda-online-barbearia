using Agenda.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Persistence;

// Dados de exemplo para rodar e demonstrar o sistema na máquina local.
// Só roda se o banco estiver vazio. Nomes genéricos de propósito (repo público).
public static class SeedDesenvolvimento
{
    public static async Task Executar(AgendaDbContext db, string senhaDev)
    {
        if (await db.Profissionais.AnyAsync() || await db.Servicos.AnyAsync())
        {
            return;
        }

        db.Servicos.AddRange(
            Servico.Criar("Corte", 30),
            Servico.Criar("Barba", 20),
            Servico.Criar("Corte + Barba", 50),
            Servico.Criar("Sobrancelha", 10));

        string[] nomes = ["Barbeiro 1", "Barbeiro 2", "Barbeiro 3", "Barbeiro 4"];
        foreach (var nome in nomes)
        {
            var login = nome.Replace(" ", "").ToLower();
            var hash = BCrypt.Net.BCrypt.HashPassword(senhaDev);
            var profissional = Profissional.Criar(nome, login, hash);
            db.Profissionais.Add(profissional);

            // Segunda a sábado, 09:00 às 20:00.
            for (var dia = DayOfWeek.Monday; dia <= DayOfWeek.Saturday; dia++)
            {
                db.Expedientes.Add(PeriodoExpediente.Criar(profissional.Id, dia, new TimeOnly(9, 0), new TimeOnly(20, 0)));
            }
        }

        await db.SaveChangesAsync();
    }
}
