using Agenda.Domain;

namespace Agenda.Application.Interface;


public interface IDisponibilidadeLeitura
{
    Task<Servico?> ObterServico(Guid servicoId);
    Task<List<PeriodoExpediente>> ObterExpedientes(Guid profissionalId, DayOfWeek dia);
    Task<List<Intervalo>> ObterOcupados(Guid profissionalId, DateTimeOffset inicioDia, DateTimeOffset fimDia);

}