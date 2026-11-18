using Agenda.Application.Exceptions;
using Agenda.Application.Interface;
using Agenda.Domain;
using Agenda.Domain.Enum;
using Microsoft.Extensions.Time.Testing;

namespace Agenda.Application.Tests;

public class CriarAgendamentoTests
{
    // Relógio: terça 10/11/2026, 10:00 em São Paulo. Reserva para quinta 12/11.
    private static readonly DateTimeOffset Agora = new(2026, 11, 10, 13, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan FusoSp = TimeSpan.FromHours(-3);
    private static readonly DateOnly Data = new(2026, 11, 12);
    private static readonly Guid ProfissionalId = Guid.CreateVersion7();

    private static LeituraFake LeituraComExpediente09as10(Servico? servico)
        => new()
        {
            Servico = servico,
            Expedientes = [PeriodoExpediente.Criar(ProfissionalId, Data.DayOfWeek, new TimeOnly(9, 0), new TimeOnly(10, 0))],
        };

    private static CriarAgendamento Sut(LeituraFake leitura, EscritaFake escrita)
    {
        var consulta = new ConsultarHorariosLivres(leitura, new FakeTimeProvider(Agora));
        return new CriarAgendamento(leitura, consulta, escrita);
    }

    [Fact]
    public async Task Criar_ServicoInexistente_LancaNotFoundENaoGrava()
    {
        var leitura = LeituraComExpediente09as10(servico: null);
        var escrita = new EscritaFake();
        var inicio = new DateTimeOffset(2026, 11, 12, 9, 0, 0, FusoSp);

        await Assert.ThrowsAsync<NotFoundException>(
            () => Sut(leitura, escrita).Executar(ProfissionalId, Guid.CreateVersion7(), inicio, "Joao", "34999990000"));

        Assert.Empty(escrita.Gravados);
    }

    [Fact]
    public async Task Criar_HorarioOcupado_LancaConflictENaoGrava()
    {
        var leitura = LeituraComExpediente09as10(Servico.Criar("Corte", 30));
        leitura.Ocupados = [Intervalo.Create(new DateTimeOffset(2026, 11, 12, 9, 0, 0, FusoSp), new DateTimeOffset(2026, 11, 12, 9, 30, 0, FusoSp))];
        var escrita = new EscritaFake();
        var inicio = new DateTimeOffset(2026, 11, 12, 9, 0, 0, FusoSp);

        await Assert.ThrowsAsync<ConflictException>(
            () => Sut(leitura, escrita).Executar(ProfissionalId, Guid.CreateVersion7(), inicio, "Joao", "34999990000"));

        Assert.Empty(escrita.Gravados);
    }

    [Fact]
    public async Task Criar_HorarioForaDaGrade_LancaConflict()
    {
        // 09:05 não está na grade de 10 em 10 minutos.
        var leitura = LeituraComExpediente09as10(Servico.Criar("Corte", 30));
        var escrita = new EscritaFake();
        var inicio = new DateTimeOffset(2026, 11, 12, 9, 5, 0, FusoSp);

        await Assert.ThrowsAsync<ConflictException>(
            () => Sut(leitura, escrita).Executar(ProfissionalId, Guid.CreateVersion7(), inicio, "Joao", "34999990000"));
    }

    [Fact]
    public async Task Criar_HorarioLivre_GravaComDadosDoServico()
    {
        var leitura = LeituraComExpediente09as10(Servico.Criar("Corte", 30));
        var escrita = new EscritaFake();
        var servicoId = Guid.CreateVersion7();
        var inicio = new DateTimeOffset(2026, 11, 12, 9, 30, 0, FusoSp);

        var agendamento = await Sut(leitura, escrita).Executar(ProfissionalId, servicoId, inicio, "Joao", "34999990000");

        Assert.Same(agendamento, Assert.Single(escrita.Gravados));
        Assert.Equal("Corte", agendamento.ServicoNome);   // nome e duração vêm do banco, não do cliente
        Assert.Equal(30, agendamento.DuracaoMin);
        Assert.Equal(inicio.AddMinutes(30), agendamento.Fim);
        Assert.Equal(Origem.Web, agendamento.Origem);
        Assert.Equal(StatusAgendamento.Confirmado, agendamento.Status);
    }

    [Fact]
    public async Task Criar_InicioEnviadoEmUtc_AceitaOMesmoInstante()
    {
        // 12:30 UTC = 09:30 em São Paulo.
        var leitura = LeituraComExpediente09as10(Servico.Criar("Corte", 30));
        var escrita = new EscritaFake();
        var inicioUtc = new DateTimeOffset(2026, 11, 12, 12, 30, 0, TimeSpan.Zero);

        await Sut(leitura, escrita).Executar(ProfissionalId, Guid.CreateVersion7(), inicioUtc, "Joao", "34999990000");

        Assert.Single(escrita.Gravados);
    }

    [Fact]
    public async Task Criar_ClienteSemNome_LancaDomainException()
    {
        var leitura = LeituraComExpediente09as10(Servico.Criar("Corte", 30));
        var escrita = new EscritaFake();
        var inicio = new DateTimeOffset(2026, 11, 12, 9, 0, 0, FusoSp);

        await Assert.ThrowsAsync<Agenda.Domain.Exceptions.DomainException>(
            () => Sut(leitura, escrita).Executar(ProfissionalId, Guid.CreateVersion7(), inicio, " ", "34999990000"));

        Assert.Empty(escrita.Gravados);
    }
}

internal class EscritaFake : IAgendamentoEscrita
{
    public List<Agendamento> Gravados { get; } = [];

    public Task AdicionarAgendamento(Agendamento agendamento)
    {
        Gravados.Add(agendamento);
        return Task.CompletedTask;
    }
}
