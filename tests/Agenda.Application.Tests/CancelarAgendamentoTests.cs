using Agenda.Application.Exceptions;
using Agenda.Domain;
using Agenda.Domain.Enum;
using Agenda.Domain.Exceptions;

namespace Agenda.Application.Tests;

public class CancelarAgendamentoTests
{
    private static Agendamento NovoAgendamento()
        => Agendamento.Criar(Guid.CreateVersion7(), Guid.CreateVersion7(), "Corte", 30, "Joao", "34999990000",
            new DateTimeOffset(2026, 11, 12, 9, 0, 0, TimeSpan.FromHours(-3)), Origem.Web);

    [Fact]
    public async Task Cancelar_AgendamentoConfirmado_MudaStatusESalva()
    {
        var escrita = new EscritaFake();
        var agendamento = NovoAgendamento();
        await escrita.AdicionarAgendamento(agendamento);

        var resultado = await new CancelarAgendamento(escrita).Executar(agendamento.Id);

        Assert.Equal(StatusAgendamento.Cancelado, resultado.Status);
        Assert.Equal(1, escrita.Salvamentos);
    }

    [Fact]
    public async Task Cancelar_Inexistente_LancaNotFound()
    {
        var escrita = new EscritaFake();

        await Assert.ThrowsAsync<NotFoundException>(() => new CancelarAgendamento(escrita).Executar(Guid.CreateVersion7()));
    }

    [Fact]
    public async Task Cancelar_DuasVezes_LancaDomainExceptionENaoSalvaDeNovo()
    {
        var escrita = new EscritaFake();
        var agendamento = NovoAgendamento();
        await escrita.AdicionarAgendamento(agendamento);
        var sut = new CancelarAgendamento(escrita);
        await sut.Executar(agendamento.Id);

        await Assert.ThrowsAsync<DomainException>(() => sut.Executar(agendamento.Id));
        Assert.Equal(1, escrita.Salvamentos);
    }
}
