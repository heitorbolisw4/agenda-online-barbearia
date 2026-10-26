using Agenda.Domain;
using Agenda.Domain.Exceptions;

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

    [Fact]
    public void Create_InicioIgualFim_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => Intervalo.Create(Hora(10), Hora(10)));
    }

    [Theory]
    //          A: início   A: fim    B: início   B: fim    esperado
    [InlineData(10, 0,      10, 50,   10, 30,     11, 0,    true)]   // 1. sobreposição parcial
    [InlineData(10, 0,      10, 30,   10, 30,     11, 0,    false)]  // 2. encostados (A termina quando B começa)
    [InlineData(10, 0,      11, 0,    10, 15,     10, 30,   true)]   // 3. B dentro de A
    [InlineData(10, 0,      10, 30,   11, 0,      11, 30,   false)]  // 4. separados, A antes
    [InlineData(11, 0,      11, 30,   10, 0,      10, 30,   false)]  // 5. separados, B antes
    public void SeSobrepoe_RetornaEsperado(
        int aIh, int aIm, int aFh, int aFm,
        int bIh, int bIm, int bFh, int bFm,
        bool esperado)
    {
        var a = Intervalo.Create(Hora(aIh, aIm), Hora(aFh, aFm));
        var b = Intervalo.Create(Hora(bIh, bIm), Hora(bFh, bFm));

        Assert.Equal(esperado, a.SeSobrepoe(b));
    }
}
