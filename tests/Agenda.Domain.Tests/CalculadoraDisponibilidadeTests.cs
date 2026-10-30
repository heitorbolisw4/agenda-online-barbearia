using Agenda.Domain.Exceptions;

namespace Agenda.Domain.Tests;

public class CalculadoraDisponibilidadeTests
{
    private static DateTimeOffset Hora(int h, int m = 0) =>
        new(2026, 11, 05, h, m, 0, TimeSpan.FromHours(-3));



    [Fact]
    public void HorariosLivres_JanelaDeUmaHora_RetornaQuatroHorarios()
    {
        // eu defino os horarios com inicio e fim
        var duracao = TimeSpan.FromMinutes(30);
        var grade = TimeSpan.FromMinutes(10);
        var limite = DateTimeOffset.MinValue;
        List<Intervalo> ocupados = [];
        List<Intervalo> janelas = [
            Intervalo.Create(Hora(9, 0), Hora(10, 0))
            
        ];
        List<Intervalo> esperado = [
            Intervalo.Create(Hora(9, 0), Hora(9, 30)),
            Intervalo.Create(Hora(9, 10), Hora(9, 40)),
            Intervalo.Create(Hora(9, 20), Hora(9, 50)),
            Intervalo.Create(Hora(9, 30), Hora(10, 0)),
        ];

        var resultado = CalculadoraDisponibilidade.HorariosLivres(janelas,   ocupados, duracao, grade, limite);
        
        Assert.Equal(esperado, resultado);

    }

    [Fact]
    public void HorariosLivres_DuasJanelas_NaoOfereceNoIntervaloEntreElas()
    {
        var duracao = TimeSpan.FromMinutes(30);
        var grade = TimeSpan.FromMinutes(10);
        var limite = DateTimeOffset.MinValue;
        List<Intervalo> ocupados = [];
        List<Intervalo> janelas = [
            Intervalo.Create(Hora(9, 0), Hora(10, 0)),
            Intervalo.Create(Hora(14, 0), Hora(15, 0)),
            
        ];
        List<Intervalo> esperado = [
            Intervalo.Create(Hora(9, 0), Hora(9, 30)),
            Intervalo.Create(Hora(9, 10), Hora(9, 40)),
            Intervalo.Create(Hora(9, 20), Hora(9, 50)),
            Intervalo.Create(Hora(9, 30), Hora(10, 0)),
            Intervalo.Create(Hora(14, 0), Hora(14, 30)),
            Intervalo.Create(Hora(14, 10), Hora(14, 40)),
            Intervalo.Create(Hora(14, 20), Hora(14, 50)),
            Intervalo.Create(Hora(14, 30), Hora(15, 0)),
        ];
        var resultado = CalculadoraDisponibilidade.HorariosLivres(janelas,   ocupados, duracao, grade, limite);
        Assert.Equal(esperado, resultado);
        
    }

    [Fact]
    public void HorariosLivres_ComOcupado_NaoOfereceHorarioQueSobrepoe()
    {
        var duracao = TimeSpan.FromMinutes(30);
        var grade = TimeSpan.FromMinutes(10);
        var limite = DateTimeOffset.MinValue;
        List<Intervalo> ocupados = [
            Intervalo.Create(Hora(10, 0), Hora(10, 30))
        ];
        List<Intervalo> janelas = [
            Intervalo.Create(Hora(9, 0), Hora(12, 0)),
            
        ];
        List<Intervalo> esperado = [
            Intervalo.Create(Hora(9, 0), Hora(9, 30)),
            Intervalo.Create(Hora(9, 10), Hora(9, 40)),
            Intervalo.Create(Hora(9, 20), Hora(9, 50)),
            Intervalo.Create(Hora(9, 30), Hora(10, 0)),
            Intervalo.Create(Hora(10, 30), Hora(11, 0)),
            Intervalo.Create(Hora(10, 40), Hora(11, 10)),
            Intervalo.Create(Hora(10, 50), Hora(11, 20)),
            Intervalo.Create(Hora(11, 0), Hora(11, 30)),
            Intervalo.Create(Hora(11, 10), Hora(11, 40)),
            Intervalo.Create(Hora(11, 20), Hora(11, 50)),
            Intervalo.Create(Hora(11, 30), Hora(12, 0)),
        ];
        var resultado = CalculadoraDisponibilidade.HorariosLivres(janelas,  ocupados, duracao, grade, limite);
        Assert.Equal(esperado, resultado);
        
    }


    [Fact]
    public void HorariosLivres_ComLimite_NaoOfereceHorarioAntesDoLimite()
    {
        var duracao = TimeSpan.FromMinutes(30);
        var grade = TimeSpan.FromMinutes(10);
        var limite = Hora(9, 35);
        List<Intervalo> ocupados = [];
        List<Intervalo> janelas = [
            Intervalo.Create(Hora(9, 0), Hora(11, 0)),
            
        ];
        List<Intervalo> esperado = [
            Intervalo.Create(Hora(9, 40), Hora(10, 10)),
            Intervalo.Create(Hora(9, 50), Hora(10, 20)),
            Intervalo.Create(Hora(10, 0), Hora(10, 30)),
            Intervalo.Create(Hora(10, 10), Hora(10, 40)),
            Intervalo.Create(Hora(10, 20), Hora(10, 50)),
            Intervalo.Create(Hora(10, 30), Hora(11, 0)),
        ];        
        var resultado = CalculadoraDisponibilidade.HorariosLivres(janelas,  ocupados, duracao, grade, limite);
        Assert.Equal(esperado, resultado);

    }

    [Fact]
    public void HorariosLivres_GradeZero_LancaExcecao()
    {
        var duracao = TimeSpan.FromMinutes(30);
        var grade = TimeSpan.Zero;
        var limite = DateTimeOffset.MinValue;
        List<Intervalo> ocupados = [];
        List<Intervalo> janelas = [
            Intervalo.Create(Hora(9, 0), Hora(11, 0)),
            
        ];
        Assert.Throws<DomainException>(() => CalculadoraDisponibilidade.HorariosLivres(janelas,  ocupados, duracao, grade, limite));
    }
}