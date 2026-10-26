using Agenda.Domain;

namespace Agenda.Domain.Tests;

public class IntervaloTests
{
    // -3h = horário de Brasília
    private static DateTimeOffset Hora(int h, int m = 0) =>
        new(2026, 10, 26, h, m, 0, TimeSpan.FromHours(-3));

    [Fact]
    public void Create_IntervaloValido_NaoLancaExcecao()
    {
        var intervalo = Intervalo.Create(Hora(10), Hora(11));

        Assert.Equal(Hora(10), intervalo.Inicio);
        Assert.Equal(Hora(11), intervalo.Fim);
    }
}