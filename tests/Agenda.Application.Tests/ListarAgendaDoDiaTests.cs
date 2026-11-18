using Agenda.Application.Interface;
using Agenda.Domain;
using Agenda.Domain.Enum;

namespace Agenda.Application.Tests;

public class ListarAgendaDoDiaTests
{
    private static readonly TimeSpan FusoSp = TimeSpan.FromHours(-3);
    private static readonly Guid ProfissionalId = Guid.CreateVersion7();

    private static Agendamento Em(int hora, int minuto)
        => Agendamento.Criar(ProfissionalId, Guid.CreateVersion7(), "Corte", 30, "Joao", "34999990000",
            new DateTimeOffset(2026, 11, 12, hora, minuto, 0, FusoSp), Origem.Web);

    [Fact]
    public async Task Listar_PedeODiaInteiroNoFusoDeSaoPaulo()
    {
        var leitura = new CatalogoFake();

        await new ListarAgendaDoDia(leitura).Executar(ProfissionalId, new DateOnly(2026, 11, 12));

        Assert.Equal(new DateTimeOffset(2026, 11, 12, 0, 0, 0, FusoSp), leitura.InicioPedido);
        Assert.Equal(new DateTimeOffset(2026, 11, 13, 0, 0, 0, FusoSp), leitura.FimPedido);
    }

    [Fact]
    public async Task Listar_OrdenaPorHorarioEEscondeCancelados()
    {
        var cancelado = Em(8, 0);
        cancelado.Cancelar();
        var leitura = new CatalogoFake { Agendamentos = [Em(15, 0), cancelado, Em(9, 0)] };

        var agenda = await new ListarAgendaDoDia(leitura).Executar(ProfissionalId, new DateOnly(2026, 11, 12));

        Assert.Equal(2, agenda.Count);
        Assert.Equal(9, agenda[0].Inicio.Hour);
        Assert.Equal(15, agenda[1].Inicio.Hour);
    }

    [Fact]
    public async Task Listar_ComCancelados_MostraTodos()
    {
        var cancelado = Em(8, 0);
        cancelado.Cancelar();
        var leitura = new CatalogoFake { Agendamentos = [Em(9, 0), cancelado] };

        var agenda = await new ListarAgendaDoDia(leitura).Executar(ProfissionalId, new DateOnly(2026, 11, 12), incluirCancelados: true);

        Assert.Equal(2, agenda.Count);
    }
}

internal class CatalogoFake : ICatalogoLeitura
{
    public List<Agendamento> Agendamentos { get; set; } = [];
    public DateTimeOffset InicioPedido { get; private set; }
    public DateTimeOffset FimPedido { get; private set; }

    public Task<List<Agendamento>> ListarAgendamentos(Guid profissionalId, DateTimeOffset inicio, DateTimeOffset fim)
    {
        InicioPedido = inicio;
        FimPedido = fim;
        return Task.FromResult(Agendamentos);
    }

    public Task<List<Servico>> ListarServicosAtivos() => Task.FromResult(new List<Servico>());

    public Task<List<Profissional>> ListarProfissionaisAtivos() => Task.FromResult(new List<Profissional>());
}
