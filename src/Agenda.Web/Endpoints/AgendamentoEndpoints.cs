using Agenda.Application;
using Agenda.Domain;

namespace Agenda.Web.Endpoints;

public record CriarAgendamentoRequest(Guid ProfissionalId, Guid ServicoId, DateTimeOffset Inicio, string ClienteNome, string ClienteTelefone);

public record AgendamentoResponse(Guid Id, Guid ProfissionalId, Guid ServicoId, string ServicoNome, DateTimeOffset Inicio, DateTimeOffset Fim, string ClienteNome, string Status)
{
    public static AgendamentoResponse De(Agendamento a)
        => new(a.Id, a.ProfissionalId, a.ServicoId, a.ServicoNome, a.Inicio, a.Fim, a.ClienteNome, a.Status.ToString());
}

// Visão do barbeiro: inclui o telefone para ele chamar o cliente.
public record ItemAgendaResponse(Guid Id, string ServicoNome, DateTimeOffset Inicio, DateTimeOffset Fim, string ClienteNome, string ClienteTelefone, string Status, string Origem);

public static class AgendamentoEndpoints
{
    public static IEndpointRouteBuilder MapAgendamentos(this IEndpointRouteBuilder app)
    {
        app.MapPost("/agendamentos", Criar);
        app.MapPost("/agendamentos/{id:guid}/cancelar", Cancelar);
        app.MapGet("/profissionais/{profissionalId:guid}/agenda", AgendaDoDia);
        return app;
    }

    public static async Task<IResult> Criar(CriarAgendamentoRequest req, CriarAgendamento criar)
    {
        if (req.ProfissionalId == Guid.Empty || req.ServicoId == Guid.Empty)
        {
            return Results.BadRequest("Todos os dados devem ser preenchidos!");
        }

        var a = await criar.Executar(req.ProfissionalId, req.ServicoId, req.Inicio, req.ClienteNome, req.ClienteTelefone);

        return Results.Created($"/agendamentos/{a.Id}", AgendamentoResponse.De(a));
    }

    public static async Task<IResult> Cancelar(Guid id, CancelarAgendamento cancelar)
    {
        var a = await cancelar.Executar(id);
        return Results.Ok(AgendamentoResponse.De(a));
    }

    public static async Task<IResult> AgendaDoDia(Guid profissionalId, DateOnly data, ListarAgendaDoDia listar, bool incluirCancelados = false)
    {
        var agenda = await listar.Executar(profissionalId, data, incluirCancelados);
        return Results.Ok(agenda.Select(a => new ItemAgendaResponse(
            a.Id, a.ServicoNome, a.Inicio, a.Fim, a.ClienteNome, a.ClienteTelefone, a.Status.ToString(), a.Origem.ToString())));
    }
}
