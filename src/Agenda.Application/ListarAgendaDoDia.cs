using Agenda.Application.Interface;
using Agenda.Domain;
using Agenda.Domain.Enum;

namespace Agenda.Application;

// Agenda do barbeiro: tudo que ele tem no dia (horário de São Paulo), em ordem.
public class ListarAgendaDoDia(ICatalogoLeitura leitura)
{
    public async Task<List<Agendamento>> Executar(Guid profissionalId, DateOnly data, bool incluirCancelados = false)
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        var inicioDia = data.ToDateTime(TimeOnly.MinValue);
        var fimDia = data.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var inicio = new DateTimeOffset(inicioDia, timezone.GetUtcOffset(inicioDia));
        var fim = new DateTimeOffset(fimDia, timezone.GetUtcOffset(fimDia));

        var agendamentos = await leitura.ListarAgendamentos(profissionalId, inicio, fim);

        return agendamentos
            .Where(a => incluirCancelados || a.Status != StatusAgendamento.Cancelado)
            .OrderBy(a => a.Inicio)
            .ToList();
    }
}
