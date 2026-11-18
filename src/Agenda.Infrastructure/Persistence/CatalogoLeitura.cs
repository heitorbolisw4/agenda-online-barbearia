using Agenda.Application.Interface;
using Agenda.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Persistence;

public class CatalogoLeitura(AgendaDbContext db) : ICatalogoLeitura
{
    public Task<List<Servico>> ListarServicosAtivos()
        => db.Servicos.AsNoTracking().Where(s => s.Ativo).OrderBy(s => s.Nome).ToListAsync();

    public Task<List<Profissional>> ListarProfissionaisAtivos()
        => db.Profissionais.AsNoTracking().Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync();

    public Task<List<Agendamento>> ListarAgendamentos(Guid profissionalId, DateTimeOffset inicio, DateTimeOffset fim)
    {
        var inicioUtc = inicio.ToUniversalTime();
        var fimUtc = fim.ToUniversalTime();

        return db.Agendamentos.AsNoTracking()
            .Where(a => a.ProfissionalId == profissionalId && a.Inicio < fimUtc && a.Fim > inicioUtc)
            .ToListAsync();
    }
}
