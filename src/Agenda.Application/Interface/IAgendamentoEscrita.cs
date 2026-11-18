using Agenda.Domain;

namespace Agenda.Application.Interface;



public interface IAgendamentoEscrita
{
    Task AdicionarAgendamento(Agendamento agendamento);
    Task<Agendamento?> ObterPorId(Guid id);
    Task Salvar();
}
