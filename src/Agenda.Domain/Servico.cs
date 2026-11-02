using Agenda.Domain.Exceptions;

namespace Agenda.Domain;



public class Servico
{
    private Servico(string nome, int duracaoMin)
    {
        Nome = nome;
        DuracaoMin = duracaoMin;
     
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Nome { get; private set; }
    public int DuracaoMin { get; private set; }
    public bool Ativo { get; private set; } = true;



    public static Servico Criar(string nome, int duracaoMin)
    {
        ValidNome(nome);
        ValidDuracao(duracaoMin);
        return new Servico(nome, duracaoMin);
    }
    private static void ValidNome(string nome)
    {
        if(string.IsNullOrWhiteSpace( nome ))
        {
            throw new DomainException("O nome do servico nao pode estar vazio!");
        }
    }
    private static void ValidDuracao(int duracaoMin)
    {
        if( duracaoMin <= 0 )
        {
            throw new DomainException("Deve informar uma duracao valida!");
        }
    }

}