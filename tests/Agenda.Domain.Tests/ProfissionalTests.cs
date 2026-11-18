using Agenda.Domain.Exceptions;

namespace Agenda.Domain.Tests;

public class ProfissionalTests
{
    [Fact]
    public void Criar_NomeValido_NasceAtivoComId()
    {
        var profissional = Profissional.Criar("  Barbeiro 1 ");

        Assert.Equal("Barbeiro 1", profissional.Nome);
        Assert.True(profissional.Ativo);
        Assert.NotEqual(Guid.Empty, profissional.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_SemNome_LancaExcecao(string? nome)
    {
        Assert.Throws<DomainException>(() => Profissional.Criar(nome!));
    }
}
