using Agenda.Application.Exceptions;
using Agenda.Application.Interface;
using Agenda.Domain;

namespace Agenda.Application
;



public class ConsultarHorariosLivres(IDisponibilidadeLeitura leitura, TimeProvider timeProvider)
{
    
    private readonly IDisponibilidadeLeitura _leitura = leitura;

    public async Task<List<Intervalo>>  Executar(Guid servicoId, Guid profissionalId, DateOnly data)
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        var now = timeProvider.GetUtcNow();
        var nowTimezone = TimeZoneInfo.ConvertTime(now, timezone);
        var today = DateOnly.FromDateTime(nowTimezone.Date);

        if(data < today || data > today.AddDays(30))
            return [];

        var servico = await _leitura.ObterServico(servicoId);
        if( servico is null || !servico.Ativo )
        {
            throw new NotFoundException("O Serviço nao foi encontrado");
        }

        var expedientes = await _leitura.ObterExpedientes(profissionalId, data.DayOfWeek);
        if(expedientes.Count == 0 )
        {
           return [];
        }


        List<Intervalo> janelas = [];
        foreach (var expediente in expedientes)
        {
            var inicio = data.ToDateTime(expediente.Inicio);
            var fim = data.ToDateTime(expediente.Fim);
            var offset = timezone.GetUtcOffset(inicio);
            
            var inicioFuso = new DateTimeOffset(inicio, offset);
            var fimFuso = new DateTimeOffset(fim, offset);
            
            janelas.Add(Intervalo.Create(inicioFuso, fimFuso));
        }
        
        var inicioDia = data.ToDateTime(TimeOnly.MinValue);
        var fimDia = data.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var offsetDia = timezone.GetUtcOffset(inicioDia);

        var inicioDiaFuso = new DateTimeOffset(inicioDia, offsetDia);
        var fimDiaFuso = new DateTimeOffset(fimDia, offsetDia);
        var ocupados = await _leitura.ObterOcupados(profissionalId, inicioDiaFuso, fimDiaFuso);
        


        var duracao = TimeSpan.FromMinutes(servico.DuracaoMin);
        var grade = TimeSpan.FromMinutes(10);
        var limite = now.AddMinutes(15);

        return CalculadoraDisponibilidade.HorariosLivres(janelas, ocupados, duracao, grade, limite);

    }
}