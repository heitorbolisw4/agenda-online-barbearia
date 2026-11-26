using Agenda.Application.Interface;
using Agenda.Infrastructure.Persistence;
using Agenda.Web.Security;
using Microsoft.EntityFrameworkCore;
using static Agenda.Web.DTO.AuthContracts;

namespace Agenda.Web.Endpoints;


public static class AutenticacaoEndpoints
{
    public static IEndpointRouteBuilder MapAutenticacao(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", Login);
        return app;
    }

    public static async Task<IResult> Login(AgendaDbContext db, IAuthService service, ProfissionalLoginRequest request, ITokenService tokenService)
    {
        if(string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Senha))
        {
            return Results.BadRequest("Deve informar todos os dados para o Login");
        }
        var user = await db.Profissionais.FirstOrDefaultAsync(x => x.Login == request.Login);
        if( user is null || !service.Conferir( request.Senha,  user.SenhaHash ) || user.Ativo == false )
        {
            return Results.Unauthorized();
        }
        var token = tokenService.GenerateToken(user);
        return Results.Ok( new {token});
    }
}