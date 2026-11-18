using Agenda.Application.Interface;

namespace Agenda.Web.Endpoints;

public record ServicoResponse(Guid Id, string Nome, int DuracaoMin);

public record ProfissionalResponse(Guid Id, string Nome);

public static class CatalogoEndpoints
{
    public static IEndpointRouteBuilder MapCatalogo(this IEndpointRouteBuilder app)
    {
        app.MapGet("/servicos", ListarServicos);
        app.MapGet("/profissionais", ListarProfissionais);
        return app;
    }

    public static async Task<IResult> ListarServicos(ICatalogoLeitura leitura)
    {
        var servicos = await leitura.ListarServicosAtivos();
        return Results.Ok(servicos.Select(s => new ServicoResponse(s.Id, s.Nome, s.DuracaoMin)));
    }

    public static async Task<IResult> ListarProfissionais(ICatalogoLeitura leitura)
    {
        var profissionais = await leitura.ListarProfissionaisAtivos();
        return Results.Ok(profissionais.Select(p => new ProfissionalResponse(p.Id, p.Nome)));
    }
}
