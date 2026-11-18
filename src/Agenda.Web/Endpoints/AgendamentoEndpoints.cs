using Agenda.Application;

namespace Agenda.Web.Endpoints;

public record CriarAgendamentoRequest(Guid ProfissionalId, Guid ServicoId, DateTimeOffset Inicio, string ClienteNome, string ClienteTelefone);

public record AgendamentoResponse(Guid Id, Guid ProfissionalId, Guid ServicoId, string ServicoNome, DateTimeOffset Inicio, DateTimeOffset Fim, string ClienteNome, string Status);

public static class AgendamentoEndpoints
{
    public static IEndpointRouteBuilder MapAgendamentos(this IEndpointRouteBuilder app)
    {
        app.MapPost("/agendamentos", Criar);
        return app;
    }

    public static async Task<IResult> Criar(CriarAgendamentoRequest req, CriarAgendamento criar)
    {
        if (req.ProfissionalId == Guid.Empty || req.ServicoId == Guid.Empty)
        {
            return Results.BadRequest("Todos os dados devem ser preenchidos!");
        }

        var a = await criar.Executar(req.ProfissionalId, req.ServicoId, req.Inicio, req.ClienteNome, req.ClienteTelefone);

        var resposta = new AgendamentoResponse(a.Id, a.ProfissionalId, a.ServicoId, a.ServicoNome, a.Inicio, a.Fim, a.ClienteNome, a.Status.ToString());
        return Results.Created($"/agendamentos/{a.Id}", resposta);
    }
}
