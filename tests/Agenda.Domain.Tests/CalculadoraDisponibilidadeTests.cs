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
        var limite = Hora(10, 0);
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

        var horarioA = CalculadoraDisponibilidade.HorariosLivres(janelas,   ocupados, duracao, grade, limite);
        
        Assert.Equal(esperado, horarioA);

    }

    [Fact]
    public void HorariosLivres_DuasJanelas_NaoOfereceNoIntervaloEntreElas()
    {
        var duracao = TimeSpan.FromMinutes(30);
        var grade = TimeSpan.FromMinutes(10);
        var limite = Hora(10, 0);
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
        var horarioB = CalculadoraDisponibilidade.HorariosLivres(janelas,   ocupados, duracao, grade, limite);
        Assert.Equal(esperado, horarioB);
        
    }
}