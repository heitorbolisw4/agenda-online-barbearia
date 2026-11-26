using Agenda.Domain.Enum;
using Agenda.Domain.Exceptions;

namespace Agenda.Domain;

public class Profissional
{
    private Profissional(string nome, string login, string senhaHash, PapelUser papelUser)
    {
        Nome = nome;
        Login = login;
        SenhaHash = senhaHash;
        Papel = papelUser;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Nome { get; private set; }
    public bool Ativo { get; private set; } = true;
    public string Login  { get; private set; }
    public string  SenhaHash { get; private set; }
    public PapelUser Papel {get; private set;} = PapelUser.Barbeiro;

    public static Profissional Criar(string nome, string login, string senhaHash, PapelUser papel = PapelUser.Barbeiro)
    {
        var nomeNormalizado = ValidNome(nome);
        ValidLogin(login);
        ValidSenhaHash(senhaHash);
        return new Profissional(nomeNormalizado, login, senhaHash, papel );
    }
    private static string ValidNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do profissional nao pode estar vazio!");
        }
        return nome.Trim();
    }

    private static void ValidSenhaHash(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new DomainException("Voce deve informar uma senha valida");
        }
    }
    private static void ValidLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new DomainException("Voce deve informar um login valido");
        }
    }
}
