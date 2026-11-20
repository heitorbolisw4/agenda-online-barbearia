using Agenda.Domain.Enum;
using Agenda.Domain.Exceptions;

namespace Agenda.Domain.Tests;


public class AgendamentoTests
{
    private static DateTimeOffset Hora(int h, int m = 0) =>
        new(2026, 11, 05, h, m, 0, TimeSpan.FromHours(-3));
    



    [Fact]
    public void Criar_DadosValidos_NasceConfirmadoComFimCalculado()
    {
        var duracaoMin = 30;
        DateTimeOffset inicio = Hora(9, 0);

        var fim = Hora(9, 30);
        
        
        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", duracaoMin, "Jose", "3499999999", inicio,  Origem.Web);
        
        var status = agendamento.Status;

        Assert.Equal(StatusAgendamento.Confirmado, status);
        Assert.Equal(fim, agendamento.Fim);


    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_NomeClienteVazio_LancaExcecao(string? nome)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, nome!, "3499999999", Hora(9, 30), Origem.Web));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_NomeServicoVazio_LancaExcecao(string? servico)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), servico!, 30, "Jose", "3499999999", Hora(9, 30),  Origem.Web));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_TelefoneClienteVazio_LancaExcecao(string? telefone)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", telefone!, Hora(9, 30),  Origem.Web));
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public void Criar_DuracaoVaziaOuNegativa_LancaExcecao(int duracao)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", duracao, "Jose", "3499999999", Hora(9, 30),  Origem.Web));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("3499")]
    [InlineData("349a99999999")]
    [InlineData("34a99999999")]
    public void Criar_Agendamento_TelefoneInvalido_LancaExcecao(string telefone)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", telefone, Hora(9, 30),  Origem.Web));
    }

    // Entrada com máscara → o Domain grava só os dígitos.
    [Theory]
    [InlineData("(34) 9999-9999", "3499999999")]
    [InlineData("(34) 99999-0000", "34999990000")]
    [InlineData("34999990000", "34999990000")]
    [InlineData("+55 (34) 99999-0000", "34999990000")]
    public void Criar_Agendamento_TelefoneValido_GravaSoDigitos(string telefone, string esperado)
    {
        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", telefone, Hora(9, 30),  Origem.Web);

        Assert.Equal(esperado, agendamento.ClienteTelefone);
    }

    [Theory]
    [InlineData("$ilva")]
    [InlineData("a")]
    [InlineData("João-@liveira")]
    [InlineData("J0ao")]
    public void Criar_Agendamento_NomeClienteInvalido_LancaExcecao(string nome)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, nome, "3499999999", Hora(9, 30),  Origem.Web));
    }


    [Theory]
    [InlineData("  João-Silva  ", "João-Silva")]
    [InlineData("  João   Silva  ", "João Silva")]
    [InlineData("Manuela D'avila", "Manuela D'avila")]
    [InlineData("Zé", "Zé")]
    public void Criar_Agendamento_NomeClienteValido_ResultadoEsperado(string nome, string esperado)
    {
        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, nome, "3499999999", Hora(9, 30),  Origem.Web);

        Assert.Equal(esperado, agendamento.ClienteNome);
    }



    [Fact]
    public void Criar_ProfissionalIdVazio_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.Empty, Guid.NewGuid(), "Corte", 30, "Jose", "3499999999", Hora(9, 30),  Origem.Web));
    }
    [Fact]
    public void Criar_ServicoIdVazio_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.Empty, "Corte", 30, "Jose", "3499999999", Hora(9, 30),  Origem.Web));
    }


    [Fact]
    public void Cancelar_Confirmado_FicaCancelado()
    {
        var status = StatusAgendamento.Cancelado;

        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", "3499999999", Hora(9, 30),  Origem.Web);

        agendamento.Cancelar();

        Assert.Equal(status, agendamento.Status);
    
    }

    [Fact]
    public void Cancelar_JaCancelado_LancaExcecao()
    {
        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", "3499999999", Hora(9, 30),  Origem.Web);
    
        agendamento.Cancelar();

        Assert.Throws<DomainException>(() => agendamento.Cancelar());
    }

    [Theory]
    [InlineData(Origem.Web)]
    [InlineData(Origem.Interno)]
    public void Criar_ComOrigem_GuardaOrigem(Origem origem)
    {

        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", "3499999999", Hora(9, 30),  origem);

        Assert.Equal(origem, agendamento.Origem);
        
    }
}