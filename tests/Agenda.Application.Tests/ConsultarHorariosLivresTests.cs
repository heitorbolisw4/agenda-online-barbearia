using Agenda.Application.Exceptions;
using Agenda.Application.Interface;
using Agenda.Domain;
using Microsoft.Extensions.Time.Testing;

namespace Agenda.Application.Tests;

public class ConsultarHorariosLivresTests
{
    // Relógio padrão: terça 10/11/2026, 13:00 UTC = 10:00 em São Paulo.
    private static readonly DateTimeOffset Agora = new(2026, 11, 10, 13, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan FusoSp = TimeSpan.FromHours(-3);

    private static PeriodoExpediente Expediente(DateOnly data, int inicioHora, int fimHora)
        => PeriodoExpediente.Criar(Guid.CreateVersion7(), data.DayOfWeek, new TimeOnly(inicioHora, 0), new TimeOnly(fimHora, 0));

    private static Task<List<Intervalo>> Consultar(LeituraFake leitura, DateOnly data, DateTimeOffset? agora = null)
    {
        var relogio = new FakeTimeProvider(agora ?? Agora);
        var sut = new ConsultarHorariosLivres(leitura, relogio);
        return sut.Executar(Guid.CreateVersion7(), Guid.CreateVersion7(), data);
    }

    [Fact]
    public async Task Consultar_ServicoInexistente_LancaNotFound()
    {
        var data = new DateOnly(2026, 11, 10);
        var leitura = new LeituraFake { Servico = null, Expedientes = [Expediente(data, 9, 18)] };

        await Assert.ThrowsAsync<NotFoundException>(() => Consultar(leitura, data));
    }

    [Fact]
    public async Task Consultar_DataDeOntem_RetornaListaVazia()
    {
        var data = new DateOnly(2026, 11, 9);
        var leitura = new LeituraFake { Servico = Servico.Criar("Corte", 30), Expedientes = [Expediente(data, 9, 18)] };

        var res = await Consultar(leitura, data);

        Assert.Empty(res);
    }

    [Fact]
    public async Task Consultar_Daqui31Dias_RetornaListaVazia()
    {
        var data = new DateOnly(2026, 12, 11);
        var leitura = new LeituraFake { Servico = Servico.Criar("Corte", 30), Expedientes = [Expediente(data, 9, 18)] };

        var res = await Consultar(leitura, data);

        Assert.Empty(res);
    }

    [Fact]
    public async Task Consultar_Daqui30Dias_RetornaHorarios()
    {
        var data = new DateOnly(2026, 12, 10);
        var leitura = new LeituraFake { Servico = Servico.Criar("Corte", 30), Expedientes = [Expediente(data, 9, 18)] };

        var res = await Consultar(leitura, data);

        Assert.NotEmpty(res);
    }

    [Fact]
    public async Task Consultar_SemExpediente_RetornaListaVazia()
    {
        var data = new DateOnly(2026, 11, 12);
        var leitura = new LeituraFake { Servico = Servico.Criar("Corte", 30), Expedientes = [] };

        var res = await Consultar(leitura, data);

        Assert.Empty(res);
    }

    [Fact]
    public async Task Consultar_ExpedienteComOcupado_RetornaHorarioNoFusoDeSaoPaulo()
    {
        // Expediente 09:00-10:00 em SP, ocupado 09:00-09:30: sobra só 09:30-10:00.
        var data = new DateOnly(2026, 11, 12);
        var ocupado = Intervalo.Create(new DateTimeOffset(2026, 11, 12, 9, 0, 0, FusoSp), new DateTimeOffset(2026, 11, 12, 9, 30, 0, FusoSp));
        var leitura = new LeituraFake
        {
            Servico = Servico.Criar("Corte", 30),
            Expedientes = [Expediente(data, 9, 10)],
            Ocupados = [ocupado],
        };

        var res = await Consultar(leitura, data);

        var esperado = Intervalo.Create(new DateTimeOffset(2026, 11, 12, 9, 30, 0, FusoSp), new DateTimeOffset(2026, 11, 12, 10, 0, 0, FusoSp));
        Assert.Equal([esperado], res);
    }

    [Fact]
    public async Task Consultar_HojeAs21h30EmSaoPaulo_UsaODiaLocalEOLimite()
    {
        // 00:30 UTC do dia 11 = 21:30 do dia 10 em SP. "Hoje" é dia 10.
        // Limite = 21:45; na grade de 10 min o primeiro horário é 21:50.
        var agora = new DateTimeOffset(2026, 11, 11, 0, 30, 0, TimeSpan.Zero);
        var data = new DateOnly(2026, 11, 10);
        var leitura = new LeituraFake { Servico = Servico.Criar("Corte", 30), Expedientes = [Expediente(data, 9, 23)] };

        var res = await Consultar(leitura, data, agora);

        Assert.Equal(5, res.Count); // 21:50, 22:00, 22:10, 22:20, 22:30
        Assert.Equal(new DateTimeOffset(2026, 11, 10, 21, 50, 0, FusoSp), res[0].Inicio);
    }
}

internal class LeituraFake : IDisponibilidadeLeitura
{
    public List<Intervalo> Ocupados { get; set; } = [];
    public List<PeriodoExpediente> Expedientes { get; set; } = [];
    public Servico? Servico { get; set; }

    public Task<List<PeriodoExpediente>> ObterExpedientes(Guid profissionalId, DayOfWeek dia)
        => Task.FromResult(Expedientes);

    public Task<List<Intervalo>> ObterOcupados(Guid profissionalId, DateTimeOffset inicioDia, DateTimeOffset fimDia)
        => Task.FromResult(Ocupados);

    public Task<Servico?> ObterServico(Guid servicoId)
        => Task.FromResult(Servico);
}
