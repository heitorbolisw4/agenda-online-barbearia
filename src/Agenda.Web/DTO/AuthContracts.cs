namespace Agenda.Web.DTO;

public class AuthContracts
{
    public record ProfissionalLoginRequest(string Login, string Senha);
}