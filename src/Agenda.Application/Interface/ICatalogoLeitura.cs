using Agenda.Domain;

namespace Agenda.Application.Interface;

// Leituras simples para o front montar as telas (listas de escolha).
public interface ICatalogoLeitura
{
    Task<List<Servico>> ListarServicosAtivos();
    Task<List<Profissional>> ListarProfissionaisAtivos();
    Task<List<Agendamento>> ListarAgendamentos(Guid profissionalId, DateTimeOffset inicio, DateTimeOffset fim);
}
