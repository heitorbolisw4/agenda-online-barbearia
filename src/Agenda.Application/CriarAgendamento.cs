using Agenda.Application.Exceptions;
using Agenda.Application.Interface;
using Agenda.Domain.Enum;
using Agenda.Domain;

namespace Agenda.Application;

public class CriarAgendamento(IDisponibilidadeLeitura leitura, ConsultarHorariosLivres consulta, IAgendamentoEscrita escrita)
{
    public async Task<Agendamento> Executar(Guid profissionalId, Guid servicoId, DateTimeOffset inicio, string clienteNome, string clienteTelefone)
    {
        var servico = await leitura.ObterServico(servicoId) ?? throw new NotFoundException("Servico Inexistente");

        // eu converto o inicio pra timezone de sp
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        var convertTmz = TimeZoneInfo.ConvertTime(inicio, timezone);
        var data = DateOnly.FromDateTime(convertTmz.Date);

        var horarios = await consulta.Executar(servicoId, profissionalId, data);

        var livre = horarios.Any(h => h.Inicio == inicio);
        if (!livre)
        {
            throw new ConflictException("Horário Indisponível!");
        }

        var agendamento = Agendamento.Criar(profissionalId, servicoId, servico.Nome, servico.DuracaoMin, clienteNome, clienteTelefone, inicio, Origem.Web);

        await escrita.AdicionarAgendamento(agendamento);
        return agendamento;

    }
}
