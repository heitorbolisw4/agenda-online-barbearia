using Agenda.Domain.Exceptions;

namespace Agenda.Domain;



public class PeriodoExpediente
{
    private PeriodoExpediente(Guid profissionalId, DayOfWeek diaSemana,  TimeOnly inicio, TimeOnly fim)
    {
        DiaSemana = diaSemana;
        ProfissionalId = profissionalId;
        Inicio = inicio;
        Fim = fim;
    }
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public DayOfWeek DiaSemana { get;  private  set; }
    public Guid ProfissionalId { get; private set; }
    public TimeOnly Inicio { get;  private  set; }
    public TimeOnly Fim { get;  private set; }

    public static PeriodoExpediente Criar(Guid profissionalId, DayOfWeek diaSemana,  TimeOnly inicio, TimeOnly fim)
    {
        ValidDiaSemana( diaSemana );
        ValidInicioFim( inicio, fim );
        ValidProfissionalId( profissionalId );
        return new PeriodoExpediente(profissionalId ,diaSemana,  inicio, fim);
    }
    private static void ValidDiaSemana( DayOfWeek diaSemana)
    {
        if( !System.Enum.IsDefined( diaSemana ) )
        {
            throw new DomainException("Deve informar um dia valido");
        }
    }
    private static void ValidInicioFim( TimeOnly inicio, TimeOnly fim )
    {
        if( fim <= inicio )
        {
            throw new DomainException("Deve informar um inicio Valido!");
        }
    }

    private static void  ValidProfissionalId( Guid profissionalId)
    {
        if( profissionalId == Guid.Empty)
        {
            throw new DomainException("Deve informar um profissional valido");
        }
    }

}