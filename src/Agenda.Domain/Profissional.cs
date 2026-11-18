using Agenda.Domain.Exceptions;

namespace Agenda.Domain;

public class Profissional
{
    private Profissional(string nome)
    {
        Nome = nome;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Nome { get; private set; }
    public bool Ativo { get; private set; } = true;

    public static Profissional Criar(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do profissional nao pode estar vazio!");
        }
        return new Profissional(nome.Trim());
    }
}
