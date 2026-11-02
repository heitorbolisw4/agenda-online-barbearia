using Agenda.Domain.Exceptions;

namespace Agenda.Domain.Tests;



public class PeriodoExpedienteTests
{
    
    private static TimeOnly Time(int h, int m) => 
        new(h, m);


    [Fact]
    public void Criar_DadosValidos_GuardaOsCampos()
    {
        var profissionalId = Guid.CreateVersion7();
        var resultado = PeriodoExpediente.Criar(profissionalId,  DayOfWeek.Friday,Time(9, 0), Time(12, 0));

        Assert.Equal(DayOfWeek.Friday, resultado.DiaSemana);
        Assert.Equal(Time(9, 0), resultado.Inicio);
        Assert.Equal(Time(12, 0), resultado.Fim);


        Assert.Equal(profissionalId, resultado.ProfissionalId);
        Assert.NotEqual(Guid.Empty, resultado.Id);    

    }


    [Fact]
    public void Criar_ComDiaDaSemanaInvalido_LancaExcecao()
    {

        Assert.Throws<DomainException>(() => PeriodoExpediente.Criar(Guid.CreateVersion7(),(DayOfWeek) 8, Time(9, 0), Time(12, 0)));
    }

    [Fact]
    public void Criar_ComInicioMaiorQueFim_LancaExcecao()
    {
    
        Assert.Throws<DomainException>(() => PeriodoExpediente.Criar(Guid.CreateVersion7(),DayOfWeek.Friday, Time(12, 0), Time(9, 0)));
        
    }

    [Fact]
    public void Criar_InicioIgualFim_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => PeriodoExpediente.Criar(Guid.CreateVersion7(),DayOfWeek.Friday, Time(9, 0), Time(9, 0)));

    
    }

    [Fact]
    public void Criar_ProfissionalIdVazio_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => PeriodoExpediente.Criar(Guid.Empty,DayOfWeek.Friday, Time(9, 0), Time(12, 0)));

    }
}