namespace Agenda.Domain;


public static class CalculadoraDisponibilidade
{
    


    public static List<Intervalo> HorariosLivres(List<Intervalo> janelas, List<Intervalo> ocupados, TimeSpan duracao, TimeSpan grade, DateTimeOffset limite )
    {
        var livres = new List<Intervalo>();
        foreach (var janela in janelas)
        {
            var s = janela.Inicio;
            while(s + duracao <= janela.Fim)
            {
                var horario = Intervalo.Create(s, s + duracao);
                var estaOcupado = ocupados.Any( o => o.SeSobrepoe( horario ) );
                if( !estaOcupado )
                {
                    livres.Add(horario);
                    
                }
                s += grade;
            
            }
        }
        return livres;
    }
}