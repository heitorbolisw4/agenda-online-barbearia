namespace Agenda.Application.Interface;


public interface IAuthService
{
    string Hash( string senha );
    bool Conferir( string senha, string hash );
}