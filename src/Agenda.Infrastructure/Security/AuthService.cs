using Agenda.Application.Interface;

namespace Agenda.Infrastructure.Security;


public class AuthService : IAuthService
{
    public bool Conferir(string senha, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(senha, hash);
    }

    public string Hash(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }
}