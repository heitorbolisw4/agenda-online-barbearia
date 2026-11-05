using Agenda.Application.Interface;
using Agenda.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Persistence;




public class DisponibilidadeLeitura(AgendaDbContext db) : IDisponibilidadeLeitura
{
    public Task<List<PeriodoExpediente>> ObterExpedientes(Guid profissionalId, DayOfWeek dia)
    {
        return db.Expedientes.Where( e => e.ProfissionalId == profissionalId && e.DiaSemana == dia ).ToListAsync();

    }

    public Task<List<Intervalo>> ObterOcupados(Guid profissionalId, DateTimeOffset inicioDia, DateTimeOffset fimDia)
    {
        return db.Agendamentos.Where( 
            o => o.ProfissionalId == profissionalId &&
            o.Status != Domain.Enum.StatusAgendamento.Cancelado &&
            o.Inicio < fimDia &&
            o.Fim >inicioDia )
            .Select( i => Intervalo.Create(inicio: i.Inicio, fim: i.Fim)).ToListAsync();
        
    }

    public Task<Servico?> ObterServico( Guid servicoId )
    {
        
        return db.Servicos.FindAsync( servicoId ).AsTask();
        

    }
}


