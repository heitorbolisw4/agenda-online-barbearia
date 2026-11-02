using Agenda.Domain.Exceptions;

namespace Agenda.Domain.Tests;

public class ServicoTests
{
    

    [Fact]
    public void Criar_DadosValidos_NasceAtivoComId()
    {
        var nome = "Corte";
        var duracaoMin = 30;

        var servico = Servico.Criar(nome, duracaoMin);

        Assert.Equal("Corte", servico.Nome);
        Assert.Equal(30, servico.DuracaoMin);

        Assert.True(servico.Ativo);
        Assert.NotEqual(Guid.Empty, servico.Id);
        

    }



    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_ServicoSemNome_LancaExcecao(string? nome)
    {
        Assert.Throws<DomainException>(() => Servico.Criar(nome!, 30));
        
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public void Criar_ServicoSemDuracao_LancaExcecao(int duracao)
    {
        Assert.Throws<DomainException>(() => Servico.Criar("Jose", duracao));
    }
}