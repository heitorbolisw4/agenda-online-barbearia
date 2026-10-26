using Agenda.Domain.Exceptions;

namespace Agenda.Domain;

public record Intervalo
{
    private Intervalo(DateTimeOffset inicio, DateTimeOffset fim)
    {
        Inicio = inicio;
        Fim = fim;
    
    }
    public DateTimeOffset Inicio { get;}
    public DateTimeOffset Fim {get;}


    public static Intervalo Create( DateTimeOffset inicio, DateTimeOffset fim )
    {
        ValidInterval( inicio, fim ); 
        return new Intervalo(inicio,  fim); 
    }


    private static void ValidInterval( DateTimeOffset inicio, DateTimeOffset fim)
    {
        if ( inicio >= fim )
        {
            throw new DomainException("Deve informar um intervalo valido!");
        }
    }


}