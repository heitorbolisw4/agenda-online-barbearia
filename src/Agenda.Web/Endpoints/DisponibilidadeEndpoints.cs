using Agenda.Application;
using Agenda.Application.Interface;

namespace Agenda.Web.Endpoints;




public static class DisponibilidadeEndpoints
{

    public static IEndpointRouteBuilder MapDisponibilidade( this IEndpointRouteBuilder app)
    {
        app.MapGet("/disponibilidade", Consultar);
        return app;
    }



    public static async Task<IResult> Consultar(Guid profissionalId, Guid servicoId, DateOnly data, ConsultarHorariosLivres consulta)
    {
        if(profissionalId == Guid.Empty || servicoId == Guid.Empty )
        {
            return Results.BadRequest("Todos os dados devem ser preenchidos!");
        }
        var resultado = await consulta.Executar(servicoId, profissionalId, data);
        return Results.Ok(resultado);
    }





}