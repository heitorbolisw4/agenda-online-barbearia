using Agenda.Domain;

namespace Agenda.Application.Interface;



public interface ITokenService
{
    public string GenerateToken(Profissional profissional);
}