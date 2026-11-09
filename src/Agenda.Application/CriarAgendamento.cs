using Agenda.Application.Interface;
using Agenda.Domain;

namespace Agenda.Application;

public class CriarAgendamento(IDisponibilidadeLeitura leitura, ConsultarHorariosLivres consulta, IAgendamentoEscrita escrita)
{
    public async Task<Agendamento> Executar(Guid profissionalId, Guid servicoId, DateTimeOffset inicio, string clienteNome, string clienteTelefone)
    {
        throw new NotImplementedException();
    }
}
