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
        
        
        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", duracaoMin, "Jose", "99999999", inicio);
        
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
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, nome!, "99999999", Hora(9, 30)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_NomeServicoVazio_LancaExcecao(string? servico)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), servico!, 30, "Jose", "99999999", Hora(9, 30)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Criar_TelefoneClienteVazio_LancaExcecao(string? telefone)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", telefone!, Hora(9, 30)));
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    public void Criar_DuracaoVaziaOuNegativa_LancaExcecao(int duracao)
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", duracao, "Jose", "99999999", Hora(9, 30)));
    }

    [Fact]
    public void Criar_ProfissionalIdVazio_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.Empty, Guid.NewGuid(), "Corte", 30, "Jose", "99999999", Hora(9, 30)));
    }
    [Fact]
    public void Criar_ServicoIdVazio_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => Agendamento.Criar(Guid.NewGuid(), Guid.Empty, "Corte", 30, "Jose", "99999999", Hora(9, 30)));
    }


    [Fact]
    public void Cancelar_Confirmado_FicaCancelado()
    {
        var status = StatusAgendamento.Cancelado;

        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", "99999999", Hora(9, 30));

        agendamento.Cancelar();

        Assert.Equal(status, agendamento.Status);
    
    }

    [Fact]
    public void Cancelar_JaCancelado_LancaExcecao()
    {
        var agendamento = Agendamento.Criar(Guid.NewGuid(), Guid.NewGuid(), "Corte", 30, "Jose", "99999999", Hora(9, 30));
    
        agendamento.Cancelar();

        Assert.Throws<DomainException>(() => agendamento.Cancelar());
    }
}