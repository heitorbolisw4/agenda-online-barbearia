using Agenda.Application.Exceptions;
using Agenda.Application.Interface;
using Agenda.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Agenda.Infrastructure.Persistence;

public class AgendamentoEscrita(AgendaDbContext db) : IAgendamentoEscrita
{
    public async Task AdicionarAgendamento(Agendamento agendamento)
    {
        db.Agendamentos.Add(agendamento);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ExclusionViolation })
        {
            // Duas reservas chegaram juntas para o mesmo horário.
            // A trava EXCLUDE do banco deixou só uma passar; esta é a perdedora.
            db.Entry(agendamento).State = EntityState.Detached;
            throw new ConflictException("Horário Indisponível!");
        }
    }
}
